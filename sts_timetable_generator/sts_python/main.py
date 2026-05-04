import logging
import os
import signal
import sys

from dotenv import load_dotenv

load_dotenv()

logging.basicConfig(
    level=os.getenv("LOG_LEVEL", "INFO").upper(),
    format="%(asctime)s [%(levelname)s] %(name)s — %(message)s",
    datefmt="%Y-%m-%dT%H:%M:%S",
    stream=sys.stdout,
)

logger = logging.getLogger(__name__)


def _handle_shutdown(signum, frame):
    logger.info("Shutdown signal received (%s), exiting...", signal.Signals(signum).name)
    sys.exit(0)


if __name__ == "__main__":
    signal.signal(signal.SIGINT, _handle_shutdown)
    signal.signal(signal.SIGTERM, _handle_shutdown)

    logger.info("Starting timetable generator service...")

    from configure import configure_scheduler

    try:
        configure_scheduler()
    except ConnectionError as e:
        logger.critical("Startup failed: %s", e)
        sys.exit(1)
    except KeyboardInterrupt:
        logger.info("Interrupted by user, shutting down")
        sys.exit(0)
    except Exception as e:
        logger.exception("Unexpected error: %s", e)
        sys.exit(1)
