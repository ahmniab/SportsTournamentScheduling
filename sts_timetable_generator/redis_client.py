import json
import logging
from redis import Redis, ConnectionError as RedisConnectionError

logger = logging.getLogger(__name__)


class RedisClient:
    def __init__(self, host="localhost", port=6379, user=None, password=None, db=0):
        self.client = Redis(
            host=host,
            port=int(port),
            username=user,
            password=password,
            db=db,
            decode_responses=True,
        )

    def ping(self):
        try:
            self.client.ping()
            logger.info("Redis connection healthy")
            return True
        except RedisConnectionError as e:
            logger.error("Redis connection failed: %s", e)
            return False

    def set(self, key, value):
        self.client.set(key, value)
        logger.debug("Redis SET key='%s'", key)

    def get(self, key):
        value = self.client.get(key)
        logger.debug("Redis GET key='%s' found=%s", key, value is not None)
        return value

    def set_json(self, key, data, ttl=None):
        serialized = json.dumps(data)
        if ttl:
            self.client.setex(key, ttl, serialized)
        else:
            self.client.set(key, serialized)
        logger.debug("Redis SET JSON key='%s'", key)

    def get_json(self, key):
        raw = self.client.get(key)
        if raw is None:
            logger.warning("Redis GET JSON key='%s' not found", key)
            return None
        return json.loads(raw)

    def get_hash(self, key):
        """
        Read a Redis hash and return a plain dict.

        Handles the .NET IDistributedCache hash layout where the actual payload
        is stored under a field named 'data' alongside expiry metadata fields
        ('absexp', 'sldexp'). When a 'data' field is present its value is
        JSON-decoded and returned directly.

        For hashes without a 'data' field each field value is JSON-decoded
        when it looks like a JSON array or object; otherwise kept as-is.
        """
        raw = self.client.hgetall(key)
        if not raw:
            logger.warning("Redis HGETALL key='%s' returned empty / not found", key)
            return None
        logger.debug("Redis HGETALL key='%s' — %d fields", key, len(raw))
        if "data" in raw:
            return json.loads(raw["data"])
        result = {}
        for field, value in raw.items():
            stripped = value.strip()
            if stripped.startswith(("{", "[")):
                try:
                    result[field] = json.loads(value)
                    continue
                except (json.JSONDecodeError, ValueError):
                    pass
            result[field] = value
        return result

    def get_smart(self, key):
        """
        Read a key regardless of whether it is a string or a hash.
        Returns a Python dict for hash keys, the raw string for string keys,
        or None when the key does not exist.
        """
        key_type = self.client.type(key)
        if key_type == "none":
            logger.warning("Redis key='%s' does not exist", key)
            return None
        if key_type == "hash":
            return self.get_hash(key)
        if key_type == "string":
            return self.get_json(key)
        logger.error("Redis key='%s' has unsupported type '%s'", key, key_type)
        return None

    def delete(self, key):
        self.client.delete(key)
        logger.debug("Redis DELETE key='%s'", key)
