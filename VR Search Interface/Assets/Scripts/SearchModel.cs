// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
using System;
using System.Collections.Generic;

public class Author
{
    public string name { get; set; }
}

public class Identifier
{
    public string identifier { get; set; }
    public string type { get; set; }
}

public class Language
{
    public string code { get; set; }
    public string name { get; set; }
}

public class Journal
{
    public object title { get; set; }
    public List<string> identifiers { get; set; }
}

public class Link
{
    public string type { get; set; }
    public string url { get; set; }
}

public class Result
{
    public DateTime? acceptedDate { get; set; }
    public string arxivId { get; set; }
    public List<Author> authors { get; set; }
    public object citationCount { get; set; }
    public List<string> contributors { get; set; }
    public List<string> outputs { get; set; }
    public DateTime createdDate { get; set; }
    public List<string> dataProviders { get; set; }
    public DateTime? depositedDate { get; set; }
    public string @abstract { get; set; }
    public string documentType { get; set; }
    public string doi { get; set; }
    public string downloadUrl { get; set; }
    public object fieldOfStudy { get; set; }
    public string fullText { get; set; }
    public int id { get; set; }
    public List<Identifier> identifiers { get; set; }
    public string title { get; set; }
    public Language language { get; set; }
    public string magId { get; set; }
    public List<string> oaiIds { get; set; }
    public DateTime publishedDate { get; set; }
    public string publisher { get; set; }
    public object pubmedId { get; set; }
    public List<object> references { get; set; }
    public List<string> sourceFulltextUrls { get; set; }
    public DateTime updatedDate { get; set; }
    public int yearPublished { get; set; }
    public List<Journal> journals { get; set; }
    public List<Link> links { get; set; }
}

public class SearchResponse
{
    public int totalHits { get; set; }
    public int limit { get; set; }
    public int offset { get; set; }
    public object scrollId { get; set; }
    public List<Result> results { get; set; }
    public object tooks { get; set; }
    public object esTook { get; set; }
}

