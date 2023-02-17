using Newtonsoft.Json;
namespace SaplingClient.Models;

public record SaplingGrammarResponseEdits(
    [property: JsonProperty("end")] 
    int End,
    
    [property: JsonProperty("error_type")] 
    string ErrorType,
    
    [property: JsonProperty("general_error_type")] 
    string GeneralErrorType,
    
    [property: JsonProperty("id")] 
    string Id,
    
    [property: JsonProperty("replacement")] 
    string Replacement,
    
    [property: JsonProperty("sentence")] 
    string Sentence,
    
    [property: JsonProperty("sentence_start")] 
    int SentenceStart,
    
    [property: JsonProperty("start")] 
    int Start
);

public record SaplingGrammarResponse(
    [property: JsonProperty("edits")] 
    IReadOnlyList<SaplingGrammarResponseEdits> Edits
);

