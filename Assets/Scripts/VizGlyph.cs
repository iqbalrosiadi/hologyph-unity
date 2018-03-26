using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SimpleJSON;

namespace HoloGlyph{

    public class VizGlyph : MonoBehaviour
    {
        public string URLFormatString = "http://localhost:3000/datatmp";
        public string Marker = "Mark-00";
        public string central_size = "0.4";
        private int m_secs_delay_per_server_wait = 5;
        bool readyForDataUpdate = false;
        bool verbose = true;
        public static string UNDEFINED = "undefined";
        public static float SIZE_UNIT_SCALE_FACTOR = 1.0f / 1.0f;    // Each unit in the specs is 1 cm.
        public static float DEFAULT_VIS_DIMS = 500.0f;
        //public Text myText;

        public JSONNode response;
        GameObject glyphPrefab = null;
        GameObject textPrefab = null;

        List<GlyphData> m_glyph_data = new List<GlyphData>();

        void Start()
            {

            // Load data from web server
            GetNewBatchOfData(m_glyph_data);

            }

        void GetNewBatchOfData(List<GlyphData> m_glyph_data)
        {
            Debug.Log("Fetching new batch of data.");
            StartCoroutine(GetBatch(m_glyph_data));
        }


        // Update is called once per frame
        void Update()
        {
            // Delete all previous objects 
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            
            VizualizeGlyph(m_glyph_data);
            if(readyForDataUpdate)
            {
                GetNewBatchOfData(m_glyph_data);
            }
        }


        void VizualizeGlyph(List<GlyphData> m_glyph_data)
        {  
           
            int count = 0;
            if (m_glyph_data != null)
            {
                foreach (GlyphData k in m_glyph_data)
                {
                    // IF GLYPH DATA IS HEIGHT THEN DO THIS
                    
                    ConstructGlyph(k, count, "1");
                    ConstructGlyph(k, count, "0");
                    ConstructText(k, count);
                    count++;
                }
            }

            //Debug.Log("visualized " + count.ToString() + " items");     
        }

        void ConstructText(GlyphData glyphdata, int counts)
        {
            float sizing;
            float.TryParse(string.Format(glyphdata.size), out sizing);
            if(Marker == glyphdata.marker){
                LoadTextGlyph("text");
                GameObject TextInstance = InstantiateGlyph(textPrefab, gameObject.transform);
                String _sensor_name = "";
                TextMesh textMesh = TextInstance.GetComponent(typeof(TextMesh)) as TextMesh;
                TextInstance.transform.localScale = new Vector3(sizing, sizing, sizing); //it'll be changed later!

                if(glyphdata.mark=="Sphere" && glyphdata.one_glyph == "0")
                { 
                  TextInstance.transform.localEulerAngles = new Vector3(90f, 0f, -180f); 
                  TextInstance.transform.localPosition += new Vector3(-0.5f, 0, 0);
                }

                if(glyphdata.mark=="Sphere")
                { 
                  TextInstance.transform.localEulerAngles = new Vector3(90f, 0f, -90f); 
                }

                if(glyphdata.mark=="Cylinder")
                { 
                  TextInstance.transform.localEulerAngles = new Vector3(90f, 90f, -90f); 
                }

                
                TextInstance.transform.localPosition = new Vector3(0.8f, 0f, 0.5f); 


                if (glyphdata.one_glyph == "1")
                {
                        foreach(GlyphData f in glyphdata.Fields)
                           {

                                _sensor_name+=f.sensor_name+" ("+f.channel+") : "+f.normal_value+"\n";
                           }
                        textMesh.text = _sensor_name;
                        Debug.Log(" Mark "+glyphdata.mark+" Text: "+textMesh.text+" oneglyph kah? "+glyphdata.one_glyph);
                        textMesh.characterSize = 0.4f;

                }
                else
                {
                    textMesh.text = glyphdata.sensor_name+" : "+glyphdata.normal_value;
                    TextInstance.transform.localPosition += new Vector3(counts, 0, 0);
                    Debug.Log("NOT ONEGLYPH Mark "+glyphdata.mark+" Text: "+glyphdata.sensor_name+" oneglyph kah? "+glyphdata.normal_value);
                    textMesh.characterSize = 0.5f;
                }
            
            
            TextInstance.transform.localPosition += new Vector3(0, 0, (TextInstance.transform.localScale[2]-TextInstance.transform.localScale[1]));
            
               

        }
    }

        void ConstructGlyph(GlyphData glyphdata, int counts, string ruler)
        {
            float val = 1.0f;
            float sizing;
            float.TryParse(string.Format(glyphdata.size), out sizing);

            Debug.Log("GLYPH : " + glyphdata.mark);
            Debug.Log("COUNTS : " + counts);

            
            if(Marker == glyphdata.marker){
                LoadGlyph(glyphdata.mark);
                GameObject glyphInstance = InstantiateGlyph(glyphPrefab, gameObject.transform);

                glyphInstance.transform.localScale = new Vector3(sizing, sizing, sizing); //it'll be changed later!
                glyphInstance.transform.localEulerAngles = new Vector3(90, 0f, 0f);
                SetChannelValue(glyphInstance, "color", glyphdata.def_color);

                Debug.Log("COLOR MARKER :"+ glyphdata.opacity);
                Debug.Log("COLOR MARKER :"+ glyphdata.def_color);
                Debug.Log("VALUE :"+ glyphdata.value);
                
                if(glyphdata.mark=="Sphere")
                { 
                  glyphInstance.transform.localPosition = new Vector3(-1f, 0f, 0f);  
                }

                if(glyphdata.mark=="Cylinder" && glyphdata.one_glyph == "1")
                { 
                  glyphInstance.transform.localPosition = new Vector3(-0.5f, 0f, -1f);  
                }

                
                if (glyphdata.one_glyph == "1")
                {
                    
                    foreach(GlyphData f in glyphdata.Fields)
                           {
                                float.TryParse(string.Format(f.value), out val);
                                if (ruler == "1")
                                {
                                    
                                    if ( f.channel == "height")
                                    { SetChannelValue(glyphInstance, "opacity", "0.5"); }

                                }
                                else
                                {
                                    SetChannelValue(glyphInstance, f.channel, f.value);
                                    if ( f.channel == "height")
                                    {        
                                        glyphInstance.transform.localPosition += new Vector3(0, 0, (glyphInstance.transform.localScale[2]-glyphInstance.transform.localScale[1]));
                                    }
                                }
                                


                           }


                }
                else
                {
                    float.TryParse(string.Format(glyphdata.value), out val);
                    if (ruler == "1")
                      {
                        if ( glyphdata.channel == "height")
                        { SetChannelValue(glyphInstance, "opacity", "0.5"); }
                      }
                      else
                      {
                          SetChannelValue(glyphInstance, glyphdata.channel, glyphdata.value);
                          if ( glyphdata.channel == "height")
                          {
                              glyphInstance.transform.localPosition += new Vector3(0, 0, (glyphInstance.transform.localScale[2]-glyphInstance.transform.localScale[1]));
                          }
                      }  
                }
                glyphInstance.transform.localPosition += new Vector3(counts, 0, 0);
                SetChannelValue(glyphInstance, "opacity", glyphdata.opacity);
                if (ruler == "1")
                      {
                        if ( glyphdata.channel != "height")
                        { glyphInstance.SetActive(false); }
                      }

            }


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

        void LoadTextGlyph(string glyphname)
        {
            textPrefab = Resources.Load("Glyphs/" + glyphname) as GameObject;

            if (textPrefab == null)
            {
                throw new System.Exception("Cannot load glyph " + glyphname);
            }
            else if (verbose)
            {
                Debug.Log("Loaded glyph " + glyphname);
            }
        }




     /* Later, need to make it more neat*/
       void ParsingJson(JSONNode response, ref List<GlyphData> m_glyph_data)
        {
            
                int num_devices = response.AsArray.Count;
                Debug.Log("Number of Device " + num_devices);
                for(int i=0; i<num_devices; i++)
                {

                    int num_sensors = response[i]["sensor"].AsArray.Count;
                    string _one_glyph = string.Format(response[i].AsObject["as_one_glyph"].Value);  
    
                    string _did = string.Format(response[i].AsObject["_id"].Value);
                    string _glyph = string.Format(response[i].AsObject["glyph"].Value);
                    GlyphData _root = new GlyphData(_did, "root"); 
                    string _marker = string.Format(response[i].AsObject["marker"].Value);

                    for(int j=0; j<num_sensors; j++)
                    {
                        String channel;
                        //float channel_value;
                        _did = string.Format(response[i]["sensor"][j].AsObject["_id"].Value);
                        string _sid = string.Format(response[i]["sensor"][j].AsObject["_id"].Value);
                        Debug.Log("jumlah sensor :" + response[i]["sensor"].AsArray.Count);

                        if(_one_glyph == "1")
                        {
                            GlyphData _child = new GlyphData(_did, _sid); 
                            //float.TryParse(string.Format(response[i]["sensor"][i]["data"][j].AsObject["value"].Value),out channel_value);
                            _child.channel = string.Format(response[i]["sensor"][j].AsObject["channel"].Value);
                            
                            float data_value;
                            float data_max;
                            float data_min;
                            float opacity;
                            float.TryParse(string.Format(response[i]["sensor"][j].AsObject["data"].Value),out data_value);
                            float.TryParse(string.Format(response[i]["sensor"][j].AsObject["max_val"].Value),out data_max);
                            float.TryParse(string.Format(response[i]["sensor"][j].AsObject["min_val"].Value),out data_min);
                            float.TryParse(string.Format(response[i]["sensor"][j].AsObject["opacity"].Value).Substring(0, string.Format(response[i]["sensor"][j].AsObject["opacity"].Value).Length-1),out opacity);

                            if (data_max < data_value) {data_max = data_value;}
                            //Debug.Log("COCACOLOR " + string.Format(response[i]["sensor"][j].AsObject["def_color"].Value));

                            _child.normal_value = data_value.ToString("R");
                            _child.sensor_name = string.Format(response[i]["sensor"][j].AsObject["sensor_name"].Value);
                            _child.value = ((data_value-(data_min))/(data_max-data_min)).ToString("R");
                            Debug.Log("Normalized value " + ((data_value-(data_min))/(data_max-data_min)).ToString("R"));
                            if(_child.channel=="color") {
                                _child.value=string.Format(response[i]["sensor"][j].AsObject["min_color"].Value) + string.Format(response[i]["sensor"][j].AsObject["max_color"].Value+ _child.value);
                                Debug.Log("INI ADALAH VALUE DARI SINGLE "+response[i]["sensor"][j].AsObject["min_color"].Value);
                            }
                            _child.one_glyph = _one_glyph;
                            _root.opacity = (opacity/100).ToString("R");
                            _root.size = central_size;
                            _root.def_color = string.Format(response[i]["sensor"][j].AsObject["def_color"].Value);
                            _root.mark = _glyph;
                            _root.marker = _marker;
                            _root.Fields.Add(_child);
                            

                        } else
                        {
                            _root = new GlyphData(_did, _sid);
                            float data_value;
                            float data_max;
                            float data_min;
                            float opacity;
                            float.TryParse(string.Format(response[i]["sensor"][j].AsObject["data"].Value),out data_value);
                            float.TryParse(string.Format(response[i]["sensor"][j].AsObject["max_val"].Value),out data_max);
                            float.TryParse(string.Format(response[i]["sensor"][j].AsObject["min_val"].Value),out data_min);
                            float.TryParse(string.Format(response[i]["sensor"][j].AsObject["opacity"].Value).Substring(0, string.Format(response[i]["sensor"][j].AsObject["opacity"].Value).Length-1),out opacity);
                            
                            //value.Substring(14, value.Length-14)
                            
                            //Debug.Log("COCACOLOR " + string.Format(response[i]["sensor"][j].AsObject["def_color"].Value));

                            if (data_max < data_value) {data_max = data_value;}
                            
                            _root.normal_value = data_value.ToString("R");
                            _root.sensor_name = string.Format(response[i]["sensor"][j].AsObject["sensor_name"].Value);
                            _root.value = ((data_value-(data_min))/(data_max-data_min)).ToString("R");
                            Debug.Log("Normalized value ROOT " + ((data_value-(data_min))/(data_max-data_min)).ToString("R"));
                            _root.mark = string.Format(response[i]["sensor"][j].AsObject["glyph"].Value);
                            _root.marker = _marker;
                            _root.def_color = string.Format(response[i]["sensor"][j].AsObject["def_color"].Value);
                            _root.opacity = (opacity/100).ToString("R");
                            _root.size = central_size;
                            Debug.Log("OPACITY "+_root.opacity); 
                            _root.channel = string.Format(response[i]["sensor"][j].AsObject["channel"].Value);
                            if(_root.channel=="color") {
                                _root.value=string.Format(response[i]["sensor"][j].AsObject["min_color"].Value) + string.Format(response[i]["sensor"][j].AsObject["max_color"].Value+ _root.value);
                            }
                            m_glyph_data.Add(_root);

                        }

                        
                        

                    } 

                    if(_one_glyph == "1")
                        {
                            _root.one_glyph = _one_glyph;
                            m_glyph_data.Add(_root);
                        }
                                        
                }


        } 

        IEnumerator GetBatch(List<GlyphData> m_glyph_data)
        {
            // This is the server request for the JSON data
            //WWW www = new WWW(string.Format(URLFormatString, stack_id, bay_id));
            WWW www = new WWW(URLFormatString);

            // This loop runs until the server request is done.
            while (!www.isDone)
            {
                readyForDataUpdate = false;
                yield return new WaitForSeconds(Convert.ToSingle(m_secs_delay_per_server_wait));
            }
            
            Debug.Log("Parsing");
            // response will contain the translated data from the server
            JSONNode response = JSON.Parse(www.text);

            // If the server responds with a 404 error, try again
            if (response == null)
            {
                Debug.Log("erroneus response received from server; trying again");
                StartCoroutine(GetBatch(m_glyph_data));
            }
            else
            {
                // Go through each item in the response and read data attributes:
                m_glyph_data.Clear();
                ParsingJson(response, ref m_glyph_data);
                readyForDataUpdate = true;

            }
        }


        public virtual void SetChannelValue(GameObject glyphPrefab, string channel,string value)
        {
            switch(channel)
            {
                case "x":
                    SetLocalPos(value, 0);
                    break;
                case "y":
                    SetLocalPos(value, 1);
                    break;
                case "z":
                    SetLocalPos(value, 2);
                    break;
                 case "width":
                    SetSize(glyphPrefab, value, 0);
                    break;
                case "height":
                    SetSize(glyphPrefab, value, 1);
                    break;
                case "depth":
                    SetSize(glyphPrefab, value, 2);
                    break;
                case "xoffset":
                    SetOffset(glyphPrefab, value, 0);
                    break;
                case "yoffset":
                    SetOffset(glyphPrefab, value, 1);
                    break;
                case "zoffset":
                    SetOffset(glyphPrefab, value, 2);
                    break;
                case "xoffsetpct":
                    SetOffsetPct(glyphPrefab, value, 0);
                    break;
                case "yoffsetpct":
                    SetOffsetPct(glyphPrefab, value, 1);
                    break;
                case "zoffsetpct":
                    SetOffsetPct(glyphPrefab, value, 2);
                    break;
                case "color":
                    SetColor(glyphPrefab, value);
                    break;
                case "opacity":
                    SetOpacity(glyphPrefab, value);
                    break;
                case "size":
                    SetMaxSize(glyphPrefab, value);
                    break;
                case "xrotation":
                    SetRotation(value, 0);
                    break;
                case "yrotation":
                    SetRotation(value, 1);
                    break;
                case "zrotation":
                    SetRotation(value, 2);
                    break;
                case "tooltip":
                    throw new System.Exception("tooltip is not a valid channel.");
                case "x2":
                    throw new System.Exception("x2 is not a valid channel - use x, and width instead.");
                case "y2":
                    throw new System.Exception("y2 is not a valid channel - use y, and height instead.");
                case "z2":
                    throw new System.Exception("z2 is not a valid channel - use z, and depth instead.");
                default:
                    throw new System.Exception("Cannot find channel: " + channel);
            }
        }

        private void SetLocalPos(string value, int dim)
        {
            // TODO: Do this more robustly.
            float pos = float.Parse(value) * HoloGlyph.VizGlyph.SIZE_UNIT_SCALE_FACTOR;

            Vector3 localPos = gameObject.transform.localPosition;
            localPos[dim] = pos;
            gameObject.transform.localPosition = localPos;
        }

         private void SetSize(GameObject glyphPrefab, string value, int dim)
        {
            float size = float.Parse(value) * HoloGlyph.VizGlyph.SIZE_UNIT_SCALE_FACTOR;

            Vector3 initPos = glyphPrefab.transform.localPosition;

            Vector3 curScale = glyphPrefab.transform.localScale;

            glyphPrefab.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            Vector3 origMeshSize = glyphPrefab.GetComponent<MeshFilter>().mesh.bounds.size;
            //Debug.Log("SET SIZE HEIGHT :"+ origMeshSize[dim]);



            curScale[dim] = size * curScale[dim];//(origMeshSize[dim]);
            Debug.Log("SET SIZE HEIGHT :"+ curScale[dim]    );
            //Debug.Log("CURSCALE :"+ curScale[dim]);
            //Debug.Log("INIT POS :"+ initPos);
            glyphPrefab.transform.localScale = curScale;

            glyphPrefab.transform.localPosition = initPos;  // This handles models that get translated with scaling.
        }

        private void SetOffset(GameObject glyphPrefab, string value, int dim)
        {
            float offset = float.Parse(value) * HoloGlyph.VizGlyph.SIZE_UNIT_SCALE_FACTOR;
            Vector3 translateBy = glyphPrefab.transform.localPosition;
            translateBy[dim] = offset - translateBy[dim];
            transform.localPosition = translateBy;
        }

        private void SetOffsetPct(GameObject glyphPrefab, string value, int dim)
        {
            glyphPrefab.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            float offset = float.Parse(value) * glyphPrefab.GetComponent<MeshFilter>().mesh.bounds.size[dim] *
                gameObject.transform.localScale[dim];
            Vector3 translateBy = glyphPrefab.transform.localPosition;
            translateBy[dim] = offset - translateBy[dim];
            glyphPrefab.transform.localPosition = translateBy;
        }

        private void SetRotation(string value, int dim)
        {
            Vector3 rot = transform.localEulerAngles;
            rot[dim] = float.Parse(value);
            transform.localEulerAngles = rot;
        }

        public void SetMaxSize(GameObject glyphPrefab, string value)
        {
            float size = float.Parse(value) * HoloGlyph.VizGlyph.SIZE_UNIT_SCALE_FACTOR;
            Debug.Log("Set Size for sphere :"+ size);

            Vector3 renderSize = glyphPrefab.transform.GetComponent<Renderer>().bounds.size;
            Vector3 localScale = glyphPrefab.transform.localScale;

            int maxIndex = 0;
            float maxSize = renderSize[maxIndex];
            for(int i = 1; i < 3; i++)
            {
                if(maxSize < renderSize[i])
                {
                    maxSize = renderSize[i];
                    maxIndex = i;
                }
            }

            float origMaxSize = renderSize[maxIndex] / localScale[maxIndex];
            Debug.Log("Set Size for origMaxSize :"+ origMaxSize);
            Debug.Log("Set Size for renderSize :"+ renderSize[maxIndex]);
            Debug.Log("Set Size for localScale :"+ localScale[maxIndex]);
            float newLocalScale = (size * origMaxSize);

            Debug.Log("newLocalScale SIZE :"+ newLocalScale);
            glyphPrefab.transform.localScale = new Vector3(newLocalScale,
                newLocalScale, newLocalScale);
        }

        private void SetColor(GameObject glyphPrefab,string value)
        {

            if(value.Length > 13)
            {
                //Debug.Log("MORE COLOR LENGTH"+value.Length + " AND THE DATA IS "+ float.Parse(value.Substring(14, value.Length-14)));

                Color color_min;
                Color color_max;
                bool colorParsed_max = ColorUtility.TryParseHtmlString(value.Substring(7, 7), out color_max);
                bool colorParsed_min = ColorUtility.TryParseHtmlString(value.Substring(0, 7), out color_min);
                Renderer renderer = glyphPrefab.transform.GetComponent<Renderer>();
                if(renderer != null)
                {
                    renderer.material.color = Color.Lerp(color_min, color_max, float.Parse(value.Substring(14, value.Length-14)));
                } else
                {
                    Debug.Log("Cannot set color of mark without renderer object.");
                }
            }
            else
            {
                //Debug.Log("LESS COLOR LENGTH"+value.Length + " AND THE DATA IS "+ value);
                Color color;
                bool colorparsed = ColorUtility.TryParseHtmlString(value.Substring(0, 7), out color);
                Renderer renderer = glyphPrefab.transform.GetComponent<Renderer>();
                if(renderer != null)
                {
                    renderer.material.color = color;//Color.Lerp(Color.green, Color.red, float.Parse(value));
                    //Debug.Log("COLOR : "+ color);
                } else
                {
                    Debug.Log("Cannot set color of mark without renderer object.");
                }
            }

            
        }

        private void SetOpacity(GameObject glyphPrefab, string value)
        {
            Renderer renderer = glyphPrefab.transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                SetRenderModeToTransparent(renderer.material);
                Color color = renderer.material.color;
                color.a = float.Parse(value);
                renderer.material.color = color;
            }
            else
            {
                Debug.Log("Cannot set opacity of mark without renderer object.");
            }
        }

        private void SetRenderModeToTransparent(Material m)
        {
            m.SetFloat("_Mode", 2);
            m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;
        }


    }
}