FROM python:3.12-slim

ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1

WORKDIR /app

RUN addgroup --system app && adduser --system --ingroup app app

COPY sts_timetable_generator/requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

COPY sts_timetable_generator/ ./

USER app

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD python -c "import os, redis, pika; r=redis.Redis(host=os.getenv('REDIS_HOST','redis'), port=int(os.getenv('REDIS_PORT','6379')), username=os.getenv('REDIS_USER') or None, password=os.getenv('REDIS_PASSWORD') or None); r.ping(); creds=pika.PlainCredentials(os.getenv('RABBITMQ_USER','guest'), os.getenv('RABBITMQ_PASSWORD','guest')); params=pika.ConnectionParameters(host=os.getenv('RABBITMQ_HOST','rabbitmq'), port=int(os.getenv('RABBITMQ_PORT','5672')), credentials=creds, heartbeat=60, blocked_connection_timeout=30); conn=pika.BlockingConnection(params); conn.close()" || exit 1

CMD ["python", "main.py"]
