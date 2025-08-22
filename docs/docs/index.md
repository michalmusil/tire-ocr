<!-- # Welcome to MkDocs

For full documentation visit [mkdocs.org](https://www.mkdocs.org).

## Commands lol

- `mkdocs new [dir-name]` - Create a new project.
- `mkdocs serve` - Start the live-reloading docs server.
- `mkdocs build` - Build the documentation site.
- `mkdocs -h` - Print help message and exit.

## Project layout

    mkdocs.yml    # The configuration file.
    docs/
        index.md  # The documentation homepage.
        ...       # Other markdown pages, images and other files. -->

# AiPipeline

AiPipeline is an extendable and scalable platform for creating and running custom pipelines made as a part of diploma thesis project. It enables running complex, multi-step pipelines at a large scale both for single input tasks and large, statistics-driven datasets. At the same time, it provides a simple interface for extending the platform functionality by adding new, custom procedures for handling almost any use-case you can think of.

## How does it work

AiPipeline runs as a microservice oriented platform consisting of one runner, which orchestrates a network of infinitely scalable nodes. Each node then provides a set of procedures, which accept an input with a given scheme and return an output with a (not necessarily) different scheme. Using the OrchestrationRunner Api, you can chain these node procedures together, each procedure's output providing an input to the next one, thus creating a complex data pipeline. You can read more about the inner workings of the AiPipeline on the [Architecture page](/architecture)
