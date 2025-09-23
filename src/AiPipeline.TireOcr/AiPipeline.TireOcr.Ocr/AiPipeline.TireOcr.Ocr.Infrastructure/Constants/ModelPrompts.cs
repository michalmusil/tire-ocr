namespace TireOcr.Ocr.Infrastructure.Constants;

public static class ModelPrompts
{
    public static string TireCodeOcrPrompt = "In the following image, there should be a picture of a car tire. On this tire, there should be an embossed tire code and tire manufacturer. Please read both the tire code and manufacturer from the image and return only these two values in format <MANUFACTURER>|<TIRE CODE>. The format of the tire code is for example: \"185/75R14 82S\", \"195/65R16C 104/102T\" or \"P215/55ZR18 95V\". Keep in mind that the \"/\" character has to be in the output. The manufacturer is a simple brand name of tire manufacturer, for example Bridgestone or Michelin. If you can't detect any tire code or manufacturer in the photo for whatever reason, just answer with letter \"N\" and nothing else.";
}

