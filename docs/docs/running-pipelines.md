# Running Pipelines

A pipeline is a sequence of procedures from different nodes, where the output of one procedure serves as the input for the next. For a pipeline to run successfully, the ApSchemes of each chained procedure must be compatible. This means a procedure's output type must match the next procedure's expected input type.

---

## 1. Identifying Available Nodes and Procedures

Before building a pipeline, you need to know which nodes and procedures are available. You can get this information by calling the Orchestration Runner's `/Nodes` endpoint. The response is a JSON array of all available nodes and their procedures, including detailed ApScheme definitions for their inputs and outputs.

**Example Response from `/Nodes`**

```json
{
  "items": [
    {
      "id": "tire-ocr-ocr",
      "procedures": [
        {
          "id": "PerformSingleOcr",
          "schemaVersion": 1,
          "input": {
            "type": "ApObject",
            "properties": {
              "detectorType": {
                "type": "ApEnum",
                "value": "",
                "supportedCases": [
                  "GoogleGemini",
                  "MistralPixtral",
                  "OpenAiGpt",
                  "GoogleCloudVision",
                  "AzureAiVision"
                ]
              },
              "image": {
                "type": "ApFile",
                "id": "00000000-0000-0000-0000-000000000000",
                "contentType": "",
                "supportedContentTypes": [
                  "image/jpeg",
                  "image/png",
                  "image/webp"
                ]
              }
            },
            "nonRequiredProperties": []
          },
          "output": {
            "type": "ApObject",
            "properties": {
              "detectedCode": {
                "type": "ApString",
                "value": ""
              },
              "estimatedCosts": {
                "type": "ApObject",
                "properties": {
                  "inputUnitCount": {
                    "type": "ApDecimal",
                    "value": 0
                  },
                  "outputUnitCount": {
                    "type": "ApDecimal",
                    "value": 0
                  },
                  "billingUnit": {
                    "type": "ApString",
                    "value": ""
                  },
                  "estimatedCost": {
                    "type": "ApDecimal",
                    "value": 0
                  },
                  "estimatedCostCurrency": {
                    "type": "ApString",
                    "value": ""
                  }
                },
                "nonRequiredProperties": []
              }
            },
            "nonRequiredProperties": ["estimatedCosts"]
          }
        }
      ]
    },
    {
      "id": "tire-ocr-postprocessing",
      "procedures": [
        {
          "id": "PerformTireCodePostprocessing",
          "schemaVersion": 1,
          "input": {
            "type": "ApString",
            "value": ""
          },
          "output": {
            "type": "ApObject",
            "properties": {
              "rawCode": {
                "type": "ApString",
                "value": ""
              },
              "postprocessedTireCode": {
                "type": "ApString",
                "value": ""
              },
              "vehicleClass": {
                "type": "ApString",
                "value": ""
              },
              "width": {
                "type": "ApDecimal",
                "value": 0
              },
              "aspectRatio": {
                "type": "ApDecimal",
                "value": 0
              },
              "construction": {
                "type": "ApString",
                "value": ""
              },
              "diameter": {
                "type": "ApDecimal",
                "value": 0
              },
              "loadIndex": {
                "type": "ApString",
                "value": ""
              },
              "speedRating": {
                "type": "ApString",
                "value": ""
              }
            },
            "nonRequiredProperties": [
              "vehicleClass",
              "width",
              "AspectRatio",
              "construction",
              "diameter",
              "loadIndex",
              "speedRating"
            ]
          }
        }
      ]
    }
  ]
}
```

In this example, the `tire-ocr-ocr` node's `PerformSingleOcr` procedure outputs an ApObject that contains a detectedCode string. The `tire-ocr-postprocessing` node's `PerformTireCodePostprocessing` procedure expects a simple ApString as input. To chain these two, we must extract the `detectedCode` string from the first procedure's output object.

---

## 2. Output Value Selectors

When a procedure's output scheme is more complex than the next procedure's input scheme, you can use an `outputValueSelector` to narrow down the output to a specific value. This ensures scheme compatibility and allows the pipeline to run. This selector is an optional parameter of each pipeline step when constructing a pipeline (more on that later).

An `outputValueSelector` is a simple string that uses dot (`.`) separated identifiers to navigate the ApScheme JSON structure

- **To select a property of ApObject**: Use the property's name. For example, to select detectedCode from an ApObject, the selector is `"detectedCode"`.
- **To select an item of ApList**: Use the item's index. For example, if `"detectedCode"` was an ApObject containing an ApList under key `"components"` and you wanted to select it's first element, the selector would be `"detectedCode.components.0"`.
- **To navigate nested objects/lists**: Chain property names and indexes with a dot. For example, to select `inputUnitCount` from the nested `estimatedCosts` object, the selector is `"estimatedCosts.inputUnitCount"`.

---

## 3. Running a Standard Pipeline

A standard pipeline runs a single workflow in the background. It's ideal for long-running or computationally intensive tasks because the API responds immediately, and you can check the results later.

To run a standard pipeline, make a `POST` request to the `/Pipelines` endpoint with a JSON body containing your pipeline definition.

#### Request Body Structure

The request body must contain two properties:

- `input`: An ApScheme object that matches the input scheme of the first procedure in your pipeline.
- `steps`: An array of objects, where each object defines a pipeline step with a `nodeId`, a `procedureId`, and an optional `outputValueSelector`.

#### Building the Request

Following our example, let's create a pipeline that uses both procedures.

First, define the `steps` array:

```json
"steps": [
  {
    "nodeId": "tire-ocr-ocr",
    "procedureId": "PerformSingleOcr",
    "outputValueSelector": "detectedCode"
  },
  {
    "nodeId": "tire-ocr-postprocessing",
    "procedureId": "PerformTireCodePostprocessing"
  }
]
```

Notice the `outputValueSelector` in the first step, which ensures that only the `detectedCode` string is passed to the next procedure.

Next, define the `input` for the first step. This must match the `PerformSingleOcr` procedure's expected input ApScheme.

For the `detectorType` property, you may chose any of the `supportedCases` of the `PerformSingleOcr` scheme and add it to the `value` property of the enum. Let's assume you've already uploaded a tire image using the `/Files/Upload` endpoint, its ID is `16cf7a41-7ff8-492a-ab05-0ad0c724df75` and its content type `image/jpeg`. Put these values into the `image` property.

![The image used in this example](img/tire-processed.png)

_Above: image used in this example_

Our input then would look like this:

```json
"input": {
  "type": "ApObject",
  "properties": {
    "detectorType": {
      "type": "ApEnum",
      "value": "GoogleGemini"
    },
    "image": {
      "type": "ApFile",
      "id": "16cf7a41-7ff8-492a-ab05-0ad0c724df75",
      "contentType": "image/jpeg"
    }
  }
}
```

**Combining these, you get the complete request body** :

```json
{
  "input": {
    "type": "ApObject",
    "properties": {
      "detectorType": {
        "type": "ApEnum",
        "value": "GoogleGemini"
      },
      "image": {
        "type": "ApFile",
        "id": "16cf7a41-7ff8-492a-ab05-0ad0c724df75",
        "contentType": "image/jpeg"
      }
    }
  },
  "steps": [
    {
      "nodeId": "tire-ocr-ocr",
      "procedureId": "PerformSingleOcr",
      "outputValueSelector": "detectedCode"
    },
    {
      "nodeId": "tire-ocr-postprocessing",
      "procedureId": "PerformTireCodePostprocessing"
    }
  ]
}
```

The server will respond immediately with a `pipelineId`, which you can use to track the pipeline's progress.

#### Analyzing Pipeline Results

You can monitor the pipeline's status and retrieve the final results by calling the `/PipelineResults/{pipelineId}` endpoint. A pipeline is considered finished when the `finishedAt` property is no longer null. The result includes the full details of each step and any failure reasons.

---

## 4. Running an Awaited Pipeline

An awaited pipeline runs a single workflow and waits for its completion before returning a response. This is useful for short-running tasks where you need the result immediately.

To run an awaited pipeline, send a `POST` request to the `/Pipelines/Awaited` endpoint. The request body is the same as a standard pipeline, with one additional mandatory property: `timeoutSeconds`. This integer specifies how long the Orchestration Runner should wait for the pipeline to finish before returning a `408 - Timeout error`.

**Example Request Body for an Awaited Pipeline**:

```json
{
  "input": { ... },
  "steps": [ ... ],
  "timeoutSeconds": 120
}
```

If the pipeline completes successfully within the timeout, the server will return the complete pipeline result in the response body. If it times out, you can still retrieve the result later using the `/PipelineResults/{pipelineId}` endpoint.

---

## 5. Running a Pipeline Batch

A pipeline batch runs the same pipeline on multiple inputs in parallel, making it ideal for processing large datasets. Batches cannot be awaited.

To run a batch, send a `POST` request to the `/Pipelines/Batch` endpoint. Instead of a single `input` property, the request body uses an `inputs` array, where each element is an ApScheme object for a single pipeline run. The steps array remains the same.

**Example Request Body for a Pipeline Batch**:

```json
{
  "inputs": [
    {
      "type": "ApObject",
      "properties": {
        "detectorType": {
          "type": "ApEnum",
          "value": "GoogleGemini"
        },
        "image": {
          "type": "ApFile",
          "id": "16cf7a41-7ff8-492a-ab05-0ad0c724df75",
          "contentType": "image/jpeg"
        }
      }
    },
    {
      "type": "ApObject",
      "properties": {
        "detectorType": {
          "type": "ApEnum",
          "value": "MistralPixtral"
        },
        "image": {
          "type": "ApFile",
          "id": "63648f5b-fe9a-46df-90d3-d0b6ccf3345e",
          "contentType": "image/jpeg"
        }
      }
    }
  ],
  "steps": [
    {
      "nodeId": "tire-ocr-ocr",
      "procedureId": "PerformSingleOcr",
      "outputValueSelector": "detectedCode"
    },
    {
      "nodeId": "tire-ocr-postprocessing",
      "procedureId": "PerformTireCodePostprocessing"
    }
  ]
}
```

The server will respond with a `pipelineBatchId`.

#### Analyzing Pipeline Batch Results

To analyze a batch, use the `/PipelineResults/Batch/{pipelineBatchId}` endpoint. The response will provide a summary of the batch's completion status, including the number of successful and failed pipelines, along with a list of each individual pipeline result.
