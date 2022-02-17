using System.Collections.Generic;

[System.Serializable]
public class SearchCollection
{
    public List<SearchQuery> data;

}

[System.Serializable]
public class SearchQuery
{
    public string resultTitle;
    public string link;
    public int textSnippet;
}