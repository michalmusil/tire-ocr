# Running Pipelines

As already mentioned on previous pages, running pipelines is about chaining several procedures from arbitrary available nodes, each procedure's output providing an input for the next procedure in line. For pipeline to start running, the schemes of the chained procedures must be compatible with each other. In other words, if output of procedure 1 is a string, it can't be followed by procedure which accepts a complex object.

## Listing available nodes and procedures

You can list all available nodes and their respective procedures using Orchestration Runner's /Nodes endpoint.

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
    }
  ]
}
```

In the above example output of the /Nodes endpoint, you can see one running node with id 'tire-ocr-ocr'. This node contains only one procedure 'PerformSingleOcr', which accepts an object with an enum and a file as an input and it outputs an object with a string 'detectedCode' and another nested object 'estimatedCosts' with some additional key-value pairs.

## Running a standard pipeline

## Running an awaited pipeline

## Running pipeline batch
