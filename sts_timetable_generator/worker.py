import threading

def create_worker_thread (worker):
    thread = threading.Thread(target=worker.run)
    thread.start()
    return thread