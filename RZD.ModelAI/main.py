from flask import Flask, request, jsonify

app = Flask(__name__)

@app.route("/predictFreePlaces", methods=["POST"])
def predictFreePlaces():
    data = request.get_json()
    # Пример логики — просто возвращаем "предсказание" (заглушку)
    result = {"prediction": "Это пример предсказания", "input": data}
    return jsonify(result)

@app.route("/predictMinPrice", methods=["POST"])
def predictMinPrice():
    data = request.get_json()
    # Пример логики — просто возвращаем "предсказание" (заглушку)
    result = {"prediction": "Это пример предсказания", "input": data}
    return jsonify(result)


@app.route("/predictMaxPrice", methods=["POST"])
def predictMaxPrice():
    data = request.get_json()
    # Пример логики — просто возвращаем "предсказание" (заглушку)
    result = {"prediction": "Это пример предсказания", "input": data}
    return jsonify(result)


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)