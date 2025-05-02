namespace TireOcr.Ocr.Infrastructure.Constants;

public static class ModelPrompts
{
    public static string TireCodeOcrPrompt = "In the following image, there should be a picture of a portion of a car tire. On this picture, there should be embossed tire code. The format is for example: \"185/75R14 82S\", \"195/65R16C 104/102T\" or \"P215/55ZR18 95V\". Keep in mind that the \"/\" character has to be in the output. Please read the tire code from the image and return only the detected code string itself (for example just \"210/60ZR15 95V\"). If you can't detect any tire code in the photo for whatever reason, just answer with letter \"N\" and nothing else.";
}