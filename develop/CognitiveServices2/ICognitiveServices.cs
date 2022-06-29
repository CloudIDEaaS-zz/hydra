using System.Threading.Tasks;

namespace CognitiveServices
{
    public interface ICognitiveServices
    {
        Task<byte[]> GenerateSpeech(string speechText);
    }
}