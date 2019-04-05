using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebStat
{
    public class TreeInfo
    {
        public int nodesOnLevel { get; set; }
        public int levelCount { get; set; }
        public int topRequestCount { get; set; }
        public List<NodeLevelInfo> nodeLevels { get; private set; }

        public TreeInfo()
        {
            nodeLevels = new List<NodeLevelInfo>();
        }

        public void AddNewLevelInfo(NodeLevelInfo levelInfo)
        {
            nodeLevels.Add(levelInfo);
        }

        public string[] getNodeNamesSequence(RequestObject request)
        {
            string[] sequence = new string[nodeLevels.Count];
            for(int i=0;i<nodeLevels.Count;i++)
            {
                if(nodeLevels[i].LevelType==LevelType.Domain)
                {
                    sequence[i] = request.Domain;
                }
                else if(nodeLevels[i].LevelType == LevelType.Group)
                {
                    if(request.Groups.Length>nodeLevels[i].Value)
                    sequence[i] = request.Groups[nodeLevels[i].Value];
                }
                else if(nodeLevels[i].LevelType == LevelType.Tag)
                {
                    sequence[i] = request.Tag;
                }
            }
            return sequence;
        }
    }
}