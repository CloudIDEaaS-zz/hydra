using CognitiveServices.Services;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace CognitiveServices
{
    public interface ICognitiveServices
    {
        Task<Stream> GenerateSpeech(string speechText, string gender = "Female", int age = 30, string ethnicity = "Asian", string emotion = "Joy", Dictionary<string, string> aspectsOut = null);
        Task<NameFromSpeechResults> GetNameFromSpeechBytes(byte[] bytes, Guid streamIdentifier);
        Task<GeneratePhotoResponse> GenerateRandomPhoto(params KeyValuePair<string, string>[] parms);
        Task<ImageAnalysis> AnalyzePhoto(Image image);
    }
}