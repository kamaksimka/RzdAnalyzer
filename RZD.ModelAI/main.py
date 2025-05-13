from fastapi import FastAPI, Request
from fastapi.responses import JSONResponse
from pydantic import BaseModel
from typing import Any, Dict
import pandas as pd
import numpy as np
import holidays
import lightgbm as lgb
import os
from datetime import datetime

app = FastAPI()

known_car_types = ['Compartment', 'Luxury', 'ReservedSeat', 'Sedentary', 'Soft']
ru_holidays = holidays.Russia()

feature_cols = [
    'travel_time_h', 'time_to_departure_h',
    'wd_sin', 'wd_cos', 'm_sin', 'm_cos', 'is_holiday',
    'is_arrival_holiday',
    'dep_hour_sin', 'dep_hour_cos',
    'arr_hour_sin', 'arr_hour_cos',
    'countPlaces',
    # закодированные carType_* будут добавлены динамически
]

class PredictRequest(BaseModel):
    TrackedRouteId: int
    StartDate: str
    EndDate: str
    CarType: str
    CountPlace: int
    ArrivalDateTime: str
    DepartureDateTime: str

def prepare(data: Dict[str, Any]):
    start = pd.to_datetime(data["StartDate"])
    end = pd.to_datetime(data["EndDate"])
    car_type = data["CarType"]
    count_places = data["CountPlace"]
    arrival = pd.to_datetime(data["ArrivalDateTime"])
    departure = pd.to_datetime(data["DepartureDateTime"])

    travel_time = (arrival - departure).total_seconds() / 3600
    timestamps = pd.date_range(start=start, end=end, freq='H')

    results = []

    for ts in timestamps:
        time_to_departure_h = (departure - ts).total_seconds() / 3600
        if time_to_departure_h < 0:
            continue

        features = {
            "travel_time_h": travel_time,
            "time_to_departure_h": time_to_departure_h,
            "wd_sin": np.sin(2 * np.pi * departure.weekday() / 7),
            "wd_cos": np.cos(2 * np.pi * departure.weekday() / 7),
            "m_sin": np.sin(2 * np.pi * (departure.month - 1) / 12),
            "m_cos": np.cos(2 * np.pi * (departure.month - 1) / 12),
            "is_holiday": int(departure.date() in ru_holidays),
            "is_arrival_holiday": int(arrival.date() in ru_holidays),
            "dep_hour_sin": np.sin(2 * np.pi * departure.hour / 24),
            "dep_hour_cos": np.cos(2 * np.pi * departure.hour / 24),
            "arr_hour_sin": np.sin(2 * np.pi * arrival.hour / 24),
            "arr_hour_cos": np.cos(2 * np.pi * arrival.hour / 24),
            "countPlaces": count_places
        }

        for ct in known_car_types:
            features[f"carType_{ct}"] = 1 if car_type == ct else 0

        results.append(features)

    return results

def make_prediction(data: PredictRequest, model_name: str):
    if data.TrackedRouteId != 7:
        return {}

    model_path = os.path.join("models", f"lgb_model_{data.TrackedRouteId}_{model_name}.txt")
    model = lgb.Booster(model_file=model_path)

    prepared_data = prepare(data.dict())
    df = pd.DataFrame(prepared_data)
    prediction = model.predict(df)

    timestamps = pd.date_range(start=pd.to_datetime(data.StartDate), end=pd.to_datetime(data.EndDate), freq='H')
    prediction_dict = {str(timestamps[i]): float(prediction[i]) for i in range(len(timestamps))}

    return prediction_dict

@app.post("/predictFreePlaces")
async def predict_free_places(request: PredictRequest):
    result = make_prediction(request, "freePlaces")
    return JSONResponse(content=result)

@app.post("/predictMinPrice")
async def predict_min_price(request: PredictRequest):
    result = make_prediction(request, "minPlaces")
    return JSONResponse(content=result)

@app.post("/predictMaxPrice")
async def predict_max_price(request: PredictRequest):
    result = make_prediction(request, "maxPlaces")
    return JSONResponse(content=result)
