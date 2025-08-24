# Ap scheme

The **ApScheme** is a set of predefined JSON structures - **ApElements**, used to represent data types within the AiPipeline platform. This universal schema allows the **Orchestration Runner** and **Nodes** to communicate seamlessly and ensures pipeline compatibility.

The ApScheme is essential for:

- **Node Communication:** Defining the input and output data schemes for node procedures.
- **User Input:** Constructing initial pipeline inputs in a universally readable format.
- **Pipeline Validation:** Allowing the Orchestration Runner to automatically validate pipeline connections.
- **Extensibility:** Providing a universal, human-readable, and JSON-parsable structure that can be easily extended.

You will encounter ApSchemes when:

- **Discovering Nodes:** Procedure input and output are defined using ApScheme.
- **Running Pipelines:** The initial input must be a valid ApScheme instance compatible with the first procedure's input.
- **Analyzing Results:** Successful pipeline results are stored in the ApScheme format.

---

## ApElement Structure

All ApElements are JSON objects at their root. While each element has its own set of properties, they all share one mandatory key: `type`. This property acts as a type discriminator and must always contain the correct string identifier for the respective element (e.g., `"type": "ApString"`).

---

## ApElement Overview

The following is a list of all available ApElements with notes on their usage and interpretation.

### 1. ApBool

Represents a simple boolean value (`true`/`false`). The value is stored in the `value` property. In a procedure I/O scheme definition, the default value is `false`.

_Example:_

```json
{
  "type": "ApBool",
  "value": true
}
```

### 2. ApString

Represents a string of characters. The value is stored in the `value` property. In a procedure I/O scheme definition, the default value is an empty string (`""`).

_Example:_

```json
{
  "type": "ApString",
  "value": "my own string"
}
```

### 3. ApInt

Represents an integer value. The value is stored in the `value` property. In a procedure I/O scheme definition, the default value is `0`.

_Example:_

```json
{
  "type": "ApInt",
  "value": 455
}
```

### 4. ApDecimal

Represents a high-precision floating-point number. The value is stored in the `value` property. In a procedure I/O scheme definition, the default value is `0`.

_Example:_

```json
{
  "type": "ApDecimal",
  "value": 3.00123345004
}
```

### 5. ApDateTime

Represents a date and time value encoded in the ISO 8601 standard. The value is stored in the `value` property. In a procedure I/O scheme definition, the default value is the current UTC date and time.

_Example:_

```json
{
  "type": "ApDateTime",
  "value": "2024-01-24T06:09:19.384957Z"
}
```

### 6. ApEnum

Represents an enumeration with a predefined set of string values. It has two specific properties:

- `supportedCases`: An array of strings defining all possible valid values for the enumeration. This property is used only in scheme definitions.
- `value`: A string representing one of the supportedCases.

When defining a procedure scheme, `value` is an empty string and `supportedCases` is populated. When providing a pipeline input, `supportedCases` is optional, but value must match one of the cases from the procedure's scheme input definition.

_Example in procedure scheme definition:_

```json
{
  "type": "ApEnum",
  "value": "",
  "supportedCases": [
    "GoogleGemini",
    "MistralPixtral",
    "OpenAiGpt",
    "GoogleCloudVision",
    "AzureAiVision"
  ]
}
```

_Example in pipeline input:_

```json
{
  "type": "ApEnum",
  "value": "GoogleGemini"
}
```

### 6. ApFile

Represents a file stored in the central FileService. This is the only supported method for passing files between nodes. It has three specific properties:

- `id`: The GUID of the file in the FileService.
- `contentType`: The MIME type of the file (e.g., `image/jpeg`).
- `supportedContentTypes`: An array of strings defining the MIME types a procedure input can accept. This property is used only in scheme definitions.

In a procedure scheme definition, `contentType` is an empty string, and `id` is an empty GUID (`00000000-0000-0000-0000-000000000000`). When providing a pipeline input, you must supply a valid `id` and `contentType` that matches a supportedContentTypes from the procedure's scheme.

_Example in procedure scheme definition:_

```json
{
  "type": "ApFile",
  "id": "00000000-0000-0000-0000-000000000000",
  "contentType": "",
  "supportedContentTypes": ["image/jpeg", "image/png", "image/webp"]
}
```

_Example in pipeline input:_

```json
{
  "type": "ApFile",
  "id": "e9ecec8e-bf34-409e-80e4-7cd1ae651c81",
  "contentType": "image/jpeg"
}
```

### 6. ApList

Represents a list of values of a common type. The list can contain any other ApElement, including nested ApList and ApObject elements. It has one specific property:

- `items`: A JSON array containing the ApElements in the list.

In a procedure scheme definition, the `items` array should contain exactly one element, which serves as a template for the list's type. In a pipeline input, all items within the list must match this template's ApElement type.

_Example in procedure scheme definition:_

```json
{
  "type": "ApList",
  "items": [
    {
      "type": "ApString",
      "value": ""
    }
  ]
}
```

_Example in pipeline input:_

```json
{
  "type": "ApList",
  "items": [
    {
      "type": "ApString",
      "value": "My value item 1"
    },
    {
      "type": "ApString",
      "value": "My value item 2"
    }
  ]
}
```

### 7. ApObject

Represents an object with key-value pairs. Keys are unique string identifiers, and values are any other ApElement. ApObjects can be arbitrarily nested. It has two specific properties:

- `properties`: A JSON object holding the inner properties of the ApObject. Keys are unique strings, and values are ApElements.
- `nonRequiredProperties`: An array of strings used in input schemes to define optional properties.

For two ApObject schemes to be compatible (e.g., a procedure output and a subsequent procedure input), the output object must contain all required keys from the input object, with matching ApElement types. Keys listed in `nonRequiredProperties` are not checked during this validation.

_Example in procedure scheme definition:_

```json
{
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
      "supportedContentTypes": ["image/jpeg", "image/png", "image/webp"]
    }
  },
  "nonRequiredProperties": ["detectorType"]
}
```

_Example in pipeline input:_

```json
{
  "type": "ApObject",
  "properties": {
    "detectorType": {
      "type": "ApEnum",
      "value": "MistralPixtral"
    },
    "image": {
      "type": "ApFile",
      "id": "1eda3a21-bf66-4abd-9bfb-15afc1691eed",
      "contentType": "image/webp"
    }
  }
}
```

_**Note:** due to mention of `detectorType` in `nonRequiredProperties`, this would also be a valid input:_

```json
{
  "type": "ApObject",
  "properties": {
    "image": {
      "type": "ApFile",
      "id": "1eda3a21-bf66-4abd-9bfb-15afc1691eed",
      "contentType": "image/webp"
    }
  }
}
```

```

```
