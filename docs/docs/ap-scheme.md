# Ap scheme

If you need to do anything regarding running or analyzing pipelines, you will encounter the Ap scheme or ApElements. Ap scheme is a set of predefined JSON structures, which correspond to data types handled by both Orchestration Runner and the nodes withing the whole system. This scheme is neccassary so that:

- Nodes can share their input/output data schemes
- Users can construct arbitrary pipeline inputs
- Orchestration Runner can evaluate pipeline validity
- And all of this stays universal, JSON-parsable, human readable and extendable

You can encounter ApSchemes when:

- Listing available nodes and their procedures, where procedure input/output is defined using ApScheme
- Running pipelines, where you must provide the initial pipeline input using Ap scheme compatible with the first procedure's input
- Analyzing pipeline results, where successful results are stored in the ApScheme

## ApElement structure

All ApElements are constructed as JSON objects at their root. Each ApElement may have it's own sets of properties, which may differ from each other. However, every ApElement shares one common property of key `type`. This property is used as a type discriminator between the ApElements and it must always contain the correst string identifier of the respective ApElement (e.g. `"type": "ApString"`).

## ApElement overview

Following is a list of all available ApElements of the Ap scheme with notes on how to properly use and interpret them.

### 1. ApBool

ApBool is a simple ApElement representing a boolean value (true/false), which it contains in the `value` property of the root object. When ApBool is a part of the procedure i/o scheme definition, it should have a default `false` value.

_Example:_

```
{
    "type": "ApBool",
    "value": true
}
```

### 2. ApString

ApElement representing an arbitrary string of characters in it's `value` property. When ApString is a part of the procedure i/o scheme definition, it should have a default value of an empty string (`""`).

_Example:_

```
{
    "type": "ApString",
    "value": "my own string"
}
```

### 3. ApInt

ApElement representing an integer value in it's `value` property. When ApInt is a part of the procedure i/o scheme definition, it should have a default value of `0`.

_Example:_

```
{
    "type": "ApInt",
    "value": 455
}
```

### 4. ApDecimal

ApElement representing a floating-point number with hight precision in it's `value` property. When ApDecimal is a part of the procedure i/o scheme definition, it should have a default value of `0`.

_Example:_

```
{
    "type": "ApDecimal",
    "value": 3.00123345004
}
```

### 5. ApDateTime

ApElement representing a DateTime value encoded in ISO 8601 standard in it's `value` property. When ApDateTime is a part of the procedure i/o scheme definition, it should have a default value of current UTC dateTime.

_Example:_

```
{
    "type": "ApDateTime",
    "value": "2024-01-24T06:09:19.384957Z"
}
```

### 6. ApEnum

ApElement representing an enumeration of string values. It's value may only be one of supported values. It has two custom properties:

- `supportedCases`: A JSON array of strings where each string represents one case which enum value can reach
- `value`: A JSON string representing one of the supported cases

The `supportedCases` property is only used by the procedure i/o scheme definition to define which values the enum can be, the `value` property itself is an empty string in this case. When adding ApEnum as a part of your pipeline input, you don't have to fill in `supportedCases`, but you have to make sure the `value` property matches one of the supported cases from the procedure scheme definition. Two ApEnums are compatible when `value` of both of the enums is equivalent to each other's `supportedCases`, however if one enum's `value` is empty (e.g. in case of procedure i/o definition), it is not checked against `supportedCases` of the other enum.

_Example in procedure scheme definition:_

```
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

```
{
    "type": "ApEnum",
    "value": "GoogleGemini"
}
```

### 6. ApFile

ApElement representing a file stored in the common **FileService**. This is the only supported way to include files in the pipelines. ApFile has three custom properties

- `id`: A string value representing the GUID of the file uploaded in FileService
- `contentType`: A string representing the MIME type of the respective file
- `supportedContentTypes`: A JSON array of strings used to define supported MIME types of node procedures's file inputs

The `supportedContentTypes` property is only used by the procedure i/o scheme definition to define which content types of ApFiles are supported on the input. In case of procedure i/o scheme definition, the `contentType` property is an empty string and the `id` property is an empty GUID (`00000000-0000-0000-0000-000000000000`). You don't have to include `supportedContentTypes` property when adding ApFile as a part of your pipeline input, however, you must provide an existing file `id` and it's respective `contentType`, which is one of the `supportedContentType`s of the input scheme you are filling out.

_Example in procedure scheme definition:_

```
{
    "type": "ApFile",
    "id": "00000000-0000-0000-0000-000000000000",
    "contentType": "",
    "supportedContentTypes": [
        "image/jpeg",
        "image/png",
        "image/webp"
    ]
}
```

_Example in pipeline input:_

```
{
    "type": "ApFile",
    "id": "e9ecec8e-bf34-409e-80e4-7cd1ae651c81",
    "contentType": "image/jpeg",
}
```

### 6. ApList

ApElement representing a list of values with a common type. The common type may be any one of other ApElement types, including the ApList itself (can be nested). The ApList has one custom property:

- `items`: A JSON array containing other ApElements of the shared list type

When ApList is a part of the procedure i/o scheme definition, the `items` property should always include just one item with the ApElement type the list holds. When entering ApList as a part of your pipeline input, you have to make sure all items within the list are the same ApElement type and that type matches the procedure input scheme's ApList first child ApElement type.

_Example in procedure scheme definition:_

```
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

```
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

ApElement representing an object with properties (key/value pairs), where key is an object-unique string identifier and value is always some type of other ApElement - this includes other ApObject and ApList, as ApObject can be arbitrarily nested. ApObject has two custom properties

- `properties`: A JSON object holding inner properties of ApObject. Key is always a unique string value and value is always some type of ApElement
- `nonRequiredProperties`: A JSON array of strings used to define properties of the ApObject, which are optional

The `nonRequiredProperties` property is only used by the procedure input scheme definition to define which properties of the input object don't have to be provided for the procedure to work properly. Orchestration Runner always does a validity check between procedure input/output schemes. For ApObjects to be compatible, the object of the first procedure's output must contain all matching keys of the ApObject of the second procedure's input AND have a matching ApElement type under that key - exception for this is if the second procedure's keys included in the `nonRequiredProperties`, which are not checked.

When ApObject is a part of the procedure i/o scheme definition, it will always contain outline of it's `properties` property with keys and corresponding ApElement values. These values will be the default procedure i/o value mentioned at each ApElement type overview. It also may contain some values in the `nonRequiredProperties` property.

When adding ApObject as a part of your pipeline input, you have to make sure to add all properties required by the procedure's input scheme with matching keys and ApElement types.

_Example in procedure scheme definition:_

```
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
            "supportedContentTypes": [
                "image/jpeg",
                "image/png",
                "image/webp"
            ]
        }
    },
    "nonRequiredProperties": ["detectorType"]
}
```

_Example in pipeline input:_

```
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

_**Note:** thanks to mention of `detectorType` in `nonRequiredProperties`, this would also be a valid input:_

```
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
