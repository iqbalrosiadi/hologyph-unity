using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;

// Describes a keyword data
public struct KeywordData
{
    public string keyword;
    public float percent;   // percentage of books with this keyword in bay

    public KeywordData(string _keyword, float _percent)
    {
        keyword = _keyword;
        percent = _percent;
    }
}

public class Visualizer : MonoBehaviour
{
    // JSON query related
    public int stack_id = 0;
    public int bay_id = 0;
    public string URLFormatString = "https://hololibrary.herokuapp.com/api/data/loc_bay/{0}/{1}";
    private int m_secs_delay_per_server_wait = 2;

    bool verbose = true;

    // This will contain the keywords data loaded from API
    List<KeywordData> m_keywords_data = new List<KeywordData>();

    GameObject glyphPrefab = null;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Start");

        string glyphname = "cube";
        LoadGlyph(glyphname);

        // Load data from web server
        StartCoroutine(GetBatch(stack_id, bay_id));
    }

    void Visualize(List<KeywordData> keywordsData)
    {
        int count = 0;
        foreach (KeywordData k in keywordsData)
        {
            // Create object for datapoint:
            GameObject glyphInstance = InstantiateGlyph(glyphPrefab, gameObject.transform);

            // Update position:
            glyphInstance.transform.localPosition = new Vector3(count, 0, 0);

            // Update size of object based on data:
            glyphInstance.transform.localScale = new Vector3(k.percent, k.percent, k.percent);
            count++;
        }

        Debug.Log("visualized " + count.ToString() + " items");
    }

    private GameObject InstantiateGlyph(GameObject glyphPrefab, Transform parentTransform)
    {
        return Instantiate(glyphPrefab, parentTransform.position,
                    parentTransform.rotation, parentTransform);
    }

    void LoadGlyph(string glyphname)
    {
        glyphPrefab = Resources.Load("Glyphs/" + glyphname) as GameObject;

        if (glyphPrefab == null)
        {
            throw new System.Exception("Cannot load glyph " + glyphname);
        }
        else if (verbose)
        {
            Debug.Log("Loaded glyph " + glyphname);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Make this more efficient; you don't have to delete everything and start from scratch

        // Delete all previous objects 
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Update visualization with updated data
        Visualize(m_keywords_data);
    }

    IEnumerator GetBatch(int stack_id, int bay_id)
    {
        // This is the server request for the JSON data
        WWW www = new WWW(string.Format(URLFormatString, stack_id, bay_id));

        // This loop runs until the server request is done.
        while (!www.isDone)
        {
            yield return new WaitForSeconds(Convert.ToSingle(m_secs_delay_per_server_wait));
        }

        Debug.Log("Parsing");
        // response will contain the translated data from the server
        JSONNode response = JSON.Parse(www.text);

        // If the server responds with a 404 error, try again
        if (response == null)
        {
            Debug.Log("erroneus response received from server; trying again");
            StartCoroutine(GetBatch(stack_id, bay_id));
        }
        else
        {
            // Go through each item in the response and read data attributes:
            int num_items_in_response = response.AsArray.Count;
            for (int i = 0; i < response.AsArray.Count; i++)
            {
                string kw = "";
                kw = response[i].AsObject["keyword"].Value;
                
                // Get fake pct data by using the y_loc of the book.
                float pct = 0.0f;
                Debug.Log("loading " + response[i].AsObject["y_loc"].Value);
                pct = float.Parse(response[i].AsObject["y_loc"].Value);
                pct = pct / 100.0f;

                Debug.Log("Read the data item: " + kw + " pct " + pct);
                m_keywords_data.Add(new KeywordData(kw, pct));
            }
        }
    }
}
