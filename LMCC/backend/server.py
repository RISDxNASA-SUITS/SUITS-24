from flask import Flask, request, send_file, jsonify
import os

app = Flask(__name__)

# LMCC -> HMD
actions = []

# HMD -> LMCC
notifications = []

# Mission Progress
state = {}
new_state = False

# Backend configuration
ROOT_FOLDER = "root"
app.config["ROOT_FOLDER"] = ROOT_FOLDER
if not os.path.exists(ROOT_FOLDER):
    os.makedirs(ROOT_FOLDER)


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
    dir_path = os.path.dirname(os.path.join(app.config["ROOT_FOLDER"], fpath))
    os.makedirs(dir_path, exist_ok=True)

    full_fpath = os.path.join(app.config["ROOT_FOLDER"], fpath)
    file.save(full_fpath)

    return jsonify({"message": "File uploaded successfully"}), 200


@app.route("/files", methods=["GET"])
def list_files():
    # Get the directory from the request parameters
    dir = request.args.get("directory", "")
    dir_path = os.path.join(app.config["ROOT_FOLDER"], dir)
    if not os.path.exists(dir_path):
        return jsonify({"message": "Directory not found"}), 404

    # Obtains the list of files
    files = os.listdir(dir_path)
    return jsonify({"files": files}), 200


@app.route("/download", methods=["GET"])
def download_file():
    fpath = request.args.get("file_path", "")
    full_fpath = os.path.join(app.config["ROOT_FOLDER"], fpath)
    if not os.path.exists(full_fpath):
        return jsonify({"message": "File not found"}), 404

    return send_file(full_fpath, as_attachment=True)


@app.route("/delete", methods=["DELETE"])
def delete_file():
    fpath = request.args.get("file_path", "")
    full_fpath = os.path.join(app.config["ROOT_FOLDER"], fpath)
    if not os.path.exists(full_fpath):
        return jsonify({"message": "File not found"}), 404

    os.remove(full_fpath)
    return jsonify({"message": "File deleted successfully"}), 200


if __name__ == "__main__":
    app.run()
