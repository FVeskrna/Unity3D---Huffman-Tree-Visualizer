using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using TMPro;

public class Tree : MonoBehaviour
{
        public AddNext CharList;

        public List<Node> nodes = new List<Node>();

        public List<GameObject> GONodes = new List<GameObject>();
        public List<GameObject> Lines = new List<GameObject>();
        public Node Root;
        public Dictionary<char, int> Frequencies = new Dictionary<char, int>();
        public Dictionary<BitArray, char> Bits = new Dictionary<BitArray, char>();
        public string list = "";
        public List<char> chars = new List<char>();
        public TMP_InputField TextField;
        public TMP_Text CodeTab;
        public TMP_Text SymbolsTab;
        public GameObject NodeVisual;
        public int maxdepth = 0;
        public float DepthStep;
        public GameObject StartSpot;
        public GameObject Content;

        public Material material;

        public float lwidth = 0;
        public float rwidth = 0;
        public float shift;

        public void Activate()
        {
            Build();
            DepthStep = 400 / SearchDepth(Root,0f);
            //BitArray bits = Encode(TextField.text);
            //TextOutput.text = Decode(bits);
            MakeBitTab(chars);
            BuildFirstTwo(Root, StartSpot.transform.position, StartSpot);
            GONodes = IterateNodes(StartSpot);
            shiftNodes();


        }
        public void Refresh()
        {
            foreach(GameObject node in GONodes)
            Destroy(node);

            foreach(GameObject Line in Lines)
            Destroy(Line);

            nodes = new List<Node>();
            GONodes = new List<GameObject>();
            Frequencies = new Dictionary<char, int>();
            Bits = new Dictionary<BitArray, char>();
            chars = new List<char>();
            list = "";
            maxdepth = 0;

            CodeTab.text = "";
            SymbolsTab.text = "";
        }
        public void shiftNodes()
        {
            foreach(GameObject node in GONodes)
            {
                if(node.TryGetComponent(out VisualNode nodevisual))
                {
                    if(node.transform.name == "left")
                    nodevisual.shiftcount = SearchRightDepth(nodevisual.node,0f);
                    else if(node.transform.name == "right")
                    nodevisual.shiftcount = SearchLeftDepth(nodevisual.node,0f);

                    nodevisual.shiftcount++;
                    
                    nodevisual.depth = (maxdepth) - nodevisual.depth + 1;
                }
            }

            foreach(GameObject node in GONodes)
            {
                if(node.TryGetComponent(out VisualNode nodevisual))
                {
                    float tempright = 0;
                    float templeft = 0;

                    if(node.transform.Find("right") != null)
                    tempright = node.transform.Find("right").GetComponent<VisualNode>().shiftcount;

                    if(node.transform.Find("left") != null)
                    templeft = node.transform.Find("left").GetComponent<VisualNode>().shiftcount;
                    
                    if(node.transform.name == "left" && tempright >= nodevisual.shiftcount)
                    nodevisual.shiftcount = tempright + 1;

                    if(node.transform.name == "right" && templeft >= nodevisual.shiftcount)
                    nodevisual.shiftcount = templeft + 1;
                }
            }

            foreach(GameObject node in GONodes)
            {
                if(node.TryGetComponent(out VisualNode nodevisual))
                {
                    float n = nodevisual.shiftcount;
                    nodevisual.shiftamount = Convert.ToInt16(Math.Pow(Convert.ToDouble(2f),Convert.ToDouble(n-1f)));

                    if(node.name == "left")
                    node.transform.position += new Vector3(-18f*nodevisual.shiftamount,0f,0f);

                    if(node.name == "right")
                    node.transform.position += new Vector3(18f*nodevisual.shiftamount,0f,0f);

                    LineRenderer Line = node.AddComponent<LineRenderer>();
                    Line.startColor = Color.black;
                    Line.endColor = Color.black;
                    Line.useWorldSpace = true;
                    Line.material = material; 
                    Line.startWidth = 2f;
                    Line.endWidth = 2f;
                    Line.SetPosition(0,new Vector3((node.transform.position.x),(node.transform.position.y+40),(node.transform.position.z)));
                    Line.SetPosition(1,new Vector3((nodevisual.parent.transform.position.x),(nodevisual.parent.transform.position.y-20),(nodevisual.parent.transform.position.z)));
                    Lines.Add(Line.gameObject);
                }
            }

            lwidth = 0;
            rwidth = 0;
            Transform current = StartSpot.transform.Find("left");

            while (current != null)
            {
                lwidth += current.GetComponent<VisualNode>().shiftamount;
                current = current.transform.Find("left");
            }

            current = StartSpot.transform.Find("right");
            while (current != null)
            {
                rwidth += current.GetComponent<VisualNode>().shiftamount;
                current = current.transform.Find("right");
            }

            shift = ((Math.Abs(lwidth - rwidth))/2)*18;

            if(lwidth > rwidth)
            shift = -shift;

            float width = lwidth + rwidth;
            float height = (DepthStep * (maxdepth)) + 125  ;

            RectTransform rt = Content.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width*36, height);
            rt.position += new Vector3(shift, -rt.position.y,0);

            StartSpot.transform.SetParent(Content.transform);
        }

        public List<GameObject> IterateNodes(GameObject Node)
        {
            List<GameObject> Templist = new List<GameObject>();

            if(Node.GetComponent<VisualNode>() != null)
            Templist.Add(Node);

            if(Node.transform.Find("right") != null)
            {
                GameObject Right = Node.transform.Find("right").gameObject;
                Templist.AddRange(IterateNodes(Right));
            }
            if(Node.transform.Find("left") != null)
            {
                GameObject Left = Node.transform.Find("left").gameObject;
                Templist.AddRange(IterateNodes(Left));
            }
            
            return Templist;
        }
        public void MakeBitTab(List<char> chars)
        {
            List<BitArray> BitArrayList = new List<BitArray>();
            for(int i = 0; i < chars.Count; i++)
            {
                List<bool> encodedsymbol = this.Root.Traverse(chars[i], new List<bool>());

                BitArray currentbit = new BitArray(encodedsymbol.ToArray());
                Bits.Add(currentbit,chars[i]);
            }


            var ordered = Bits.OrderBy(x => x.Key.Length).ToDictionary(x => x.Key, x => x.Value);

            foreach (KeyValuePair<BitArray, char> bits in ordered)
            {
                SymbolsTab.text += bits.Value.ToString() + "\n";
                CodeTab.text += BitToString(bits.Key) + "\n";
            }

        }
        
        private void Build()
        {
            List<TMP_InputField> Chars = CharList.Chars;
            List<TMP_InputField> Freq = CharList.Freq;

            for (int i = 0; i < (Chars.Count); i++)
            {
                if (!Frequencies.ContainsKey(char.Parse(Chars[i].text)))
                {
                    Frequencies.Add(char.Parse(Chars[i].text),int.Parse(Freq[i].text));
                    chars.Add(char.Parse(Chars[i].text));
                }
            }

            foreach (KeyValuePair<char, int> symbol in Frequencies)
            {
                nodes.Add(new Node() { Symbol = symbol.Key, Frequency = symbol.Value });
            }

            while (nodes.Count > 1)
            {
                List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                if (orderedNodes.Count >= 2)
                {
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                    Node parent = new Node()
                    {
                        Symbol = '*',
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }
                this.Root = nodes.FirstOrDefault();
            }

        }
        
        



        public BitArray Encode(string source)
        {
            List<bool> encodedSource = new List<bool>();

            for (int j = 0; j < source.Length; j++)
            {
                List<bool> encodedSymbol = this.Root.Traverse(source[j], new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            BitArray bits = new BitArray(encodedSource.ToArray());


            
            return bits;
        }

        private string BitToString(BitArray bits)
        {
            string output = null;

            foreach (bool bit in bits)
            {
                output += ((bit ? 1 : 0) + "");
            }
            return output;
        }

        /*
        public string Decode(BitArray bits)
        {
            Node current = this.Root;
            string decoded = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (IsLeaf(current))
                {
                    decoded += current.Symbol;
                    current = this.Root;
                }
            }

            return decoded;
        }
        */
        public void BuildFirstTwo(Node startNode, Vector3 location,GameObject parent)
        {

            Vector3 LeftchildPos = new Vector3(parent.transform.position.x, parent.transform.position.y - DepthStep, parent.transform.position.z);
            Vector3 RightchildPos = new Vector3(parent.transform.position.x, parent.transform.position.y - DepthStep, parent.transform.position.z);
            GameObject RightNodes = new GameObject("RightNodes");
            GameObject LeftNodes = new GameObject("LeftNodes");

            GameObject Node = Instantiate(NodeVisual,location, NodeVisual.transform.rotation);
            Node.transform.parent = parent.transform;
            
            VisualNode visual = Node.GetComponent<VisualNode>();
            visual.freq = startNode.Frequency;
            visual.symbol = startNode.Symbol.ToString();
            visual.parent = parent;
            visual.node = startNode;
            visual.Index.text = "";

            if(startNode.Left != null)
            {
            LeftNodes.transform.position = LeftchildPos;
            BuildTree(startNode.Left, LeftchildPos,StartSpot,false,0);
            }
            if(startNode.Right != null)
            {
            RightNodes.transform.position = RightchildPos;
            BuildTree(startNode.Right, RightchildPos, StartSpot,true,0);
            }
        }
        public void BuildTree(Node startNode, Vector3 location,GameObject parent, bool isright, int currentdepth)
        {
            Vector3 LeftchildPos = new Vector3((location.x), (location.y - DepthStep), location.z);
            Vector3 RightchildPos = new Vector3((location.x), (location.y - DepthStep), location.z); 
            
            int depth = currentdepth;
            depth++;
            if(depth>maxdepth)
            maxdepth = depth;


            GameObject LeftNode = Instantiate(NodeVisual,location, NodeVisual.transform.rotation);
            if(isright)
            LeftNode.name = "right";
            else
            LeftNode.name = "left";
            

            VisualNode visual = LeftNode.GetComponent<VisualNode>();
            visual.freq = startNode.Frequency;
            visual.symbol = startNode.Symbol.ToString();
            visual.depth = depth;
            visual.parent = parent;
            visual.node = startNode;

            if(isright)
            visual.Index.text = "1";
            else
            visual.Index.text = "0";
            
            LeftNode.transform.parent = parent.transform;

            if(startNode.Right != null)
            BuildTree(startNode.Right, RightchildPos,LeftNode, true,depth);

            if(startNode.Left != null)
            BuildTree(startNode.Left, LeftchildPos, LeftNode, false,depth);

            
        }

        public float SearchDepth(Node node, float currentdepth)
        {
            if(node.Left != null || node.Right != null)
            currentdepth++;
            else
            return currentdepth = 0;

            float depth1 = 0;
            float depth2 = 0;

            if(node.Left != null)
            {
                depth1 =currentdepth + SearchDepth(node.Left, 0);
            }
            if(node.Right != null)
            {
                depth2 =currentdepth + SearchDepth(node.Right, 0);
            }

            if(depth1 >= depth2)
            return depth1;
            else
            return depth2;
        }


        public float SearchLeftDepth(Node node, float currentdepth)
        {
            if(node.Left != null)
            currentdepth++;
            else
            return currentdepth = 0;

            float depth = 0;

            if(node.Left != null)
                depth =currentdepth + SearchLeftDepth(node.Left, 0);

            return depth;
        }

        public float SearchRightDepth(Node node, float currentdepth)
        {
            if(node.Right != null)
            currentdepth++;
            else
            return currentdepth = 0;

            float depth = 0;

  
            if(node.Right != null)
                depth = currentdepth + SearchRightDepth(node.Right, 0);

            return depth;
        }
        /*
        private float ShiftParentCount(Node Parent,GameObject MotherNote, bool left)
        {
            Node Current = Parent;
            float count = 0f;
            if(left)
            {
                while(Current.Right != null)
                {
                    
                    Current = Current.Right;
                    count += 1;
                }

                if(Parent.Right != null && MotherNote.transform.Find("right") != null)
                    count += ShiftParentCount(Parent.Right,MotherNote.transform.Find("right").gameObject,true);

            }
            else
            {
                while(Current.Left != null)
                {
                    

                    Current = Current.Left;
                    count += 1;
                    
                }

                if(Parent.Left != null && MotherNote.transform.Find("left")!= null)
                    count += ShiftParentCount(Parent.Left,MotherNote.transform.Find("left").gameObject,false);
            }
            
            VisualNode visual = MotherNote.GetComponent<VisualNode>();
            visual.shiftcount = count;
            return count;
        }
        */


        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

}