from flask import Flask, request, send_file, jsonify
from flask_cors import CORS
import json
import os

app = Flask(__name__)
CORS(app)

# LMCC -> HMD
actions = []

# HMD -> LMCC
notifications = [
    # Mock
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
    {"message": "Begin EVA", "timestamp": "00:00:00"},
]

# Mission Progress
state = {}
new_state = False

# Backend configuration
EVA_NUM = 1
FS_ROOT = "root"
MISSION_FOLDER = "mission"
if not os.path.exists(FS_ROOT):
    os.makedirs(FS_ROOT)


@app.route("/get-tasks", methods=["GET"])
def get_tasks():
    task_titles = ["egress"]
    tasks = []

    for title in task_titles:
        with open(f"{MISSION_FOLDER}/{title}.json", "r") as f:
            tasks.append(json.load(f))

    return jsonify(tasks), 200


@app.route("/post-action", methods=["POST"])
def post_action():
    global actions
    data = request.get_json()
    actions.append(data)
    return jsonify({"message": "Received action"}), 200


@app.route("/get-actions", methods=["GET"])
def get_actions():
    global actions
    data = jsonify(actions)
    actions.clear()
    return data


@app.route("/get-state", methods=["GET"])
def get_state():
    global state
    global new_state

    if new_state:
        new_state = False
        return jsonify(state)
    else:
        return jsonify({})


@app.route("/post-notification", methods=["POST"])
def post_notification():
    global notifications
    data = request.get_json()
    notifications.append(data)
    return jsonify({"message": "Received notification"}), 200


@app.route("/get-notifications", methods=["GET"])
def get_notifications():
    global notifications
    return jsonify(notifications)


@app.route("/upload", methods=["POST"])
def upload_file():
    if "file" not in request.files:
        return jsonify({"message": "No file uploaded"}), 400

    file = request.files["file"]
    fname = file.filename
    if fname == "":
        return jsonify({"message": "No file selected"}), 400

    # Get the file path from the request parameters
    fpath = request.form.get("file_path", "")

    # Create the directory if it doesn't exist
    dir_path = os.path.dirname(os.path.join(FS_ROOT, fpath))
    os.makedirs(dir_path, exist_ok=True)

    full_fpath = os.path.join(FS_ROOT, fpath)
    file.save(full_fpath)

    return jsonify({"message": "File uploaded successfully"}), 200


@app.route("/files", methods=["GET"])
def list_files():
    # Get the directory from the request parameters
    dir = request.args.get("directory", "")
    dir_path = os.path.join(FS_ROOT, dir)
    if not os.path.exists(dir_path):
        return jsonify({"message": "Directory not found"}), 404

    # Obtains the list of files
    files = os.listdir(dir_path)
    return jsonify({"files": files}), 200


@app.route("/download", methods=["GET"])
def download_file():
    fpath = request.args.get("file_path", "")
    full_fpath = os.path.join(FS_ROOT, fpath)
    if not os.path.exists(full_fpath):
        return jsonify({"message": "File not found"}), 404

    return send_file(full_fpath, as_attachment=True)


@app.route("/delete", methods=["DELETE"])
def delete_file():
    fpath = request.args.get("file_path", "")
    full_fpath = os.path.join(FS_ROOT, fpath)
    if not os.path.exists(full_fpath):
        return jsonify({"message": "File not found"}), 404

    os.remove(full_fpath)
    return jsonify({"message": "File deleted successfully"}), 200


# Mock TSS
tss = {
    "telemetry": {
        "eva_time": 4,
        "eva1": {
            "batt_time_left": 4184.880859,
            "oxy_pri_storage": 15.204723,
            "oxy_sec_storage": 24.481083,
            "oxy_pri_pressure": 0.000000,
            "oxy_sec_pressure": 734.434692,
            "oxy_time_left": 4286,
            "heart_rate": 90.000000,
            "oxy_consumption": 0.103577,
            "co2_production": 0.104614,
            "suit_pressure_oxy": 3.072349,
            "suit_pressure_co2": 0.001071,
            "suit_pressure_other": 11.554200,
            "suit_pressure_total": 14.627621,
            "fan_pri_rpm": 0.000000,
            "fan_sec_rpm": 29855.638672,
            "helmet_pressure_co2": 0.005966,
            "scrubber_a_co2_storage": 0.000000,
            "scrubber_b_co2_storage": 0.497545,
            "temperature": 72.085022,
            "coolant_ml": 21.814507,
            "coolant_gas_pressure": 0.000000,
            "coolant_liquid_pressure": 120.961479,
        },
        "eva2": {
            "batt_time_left": 5393.291016,
            "oxy_pri_storage": 16.324224,
            "oxy_sec_storage": 17.517467,
            "oxy_pri_pressure": 0.002680,
            "oxy_sec_pressure": 525.527954,
            "oxy_time_left": 3654,
            "heart_rate": 90.000000,
            "oxy_consumption": 0.098664,
            "co2_production": 0.095775,
            "suit_pressure_oxy": 3.072346,
            "suit_pressure_cO2": 0.001163,
            "suit_pressure_other": 11.554200,
            "suit_pressure_total": 14.627710,
            "fan_pri_rpm": 0.000000,
            "fan_sec_rpm": 29433.216797,
            "helmet_pressure_co2": 0.005640,
            "scrubber_a_co2_storage": 0.000000,
            "scrubber_b_co2_storage": 0.497686,
            "temperature": 76.830681,
            "coolant_ml": 21.773628,
            "coolant_gas_pressure": 0.000000,
            "coolant_liquid_pressure": 128.683289,
        },
    }
}


@app.route("/get-tss", methods=["GET"])
def get_tss():
    global tss
    return jsonify(tss), 200


if __name__ == "__main__":
    app.run()
