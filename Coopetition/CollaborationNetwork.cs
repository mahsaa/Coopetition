using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coopetition
{
    public class CollaborationNetwork
    {
        private int id;
        private int communityId;
       // private Dictionary<int, bool> members; // <WebserviceId, IsCompetitive>
        private List<int> membersIds;

        public int Id 
        {
            get { return id; }
            set { id = value; } 
        }

        public int CommunityId 
        {
            get { return communityId; }
            set { communityId = value; } 
        }

        //public Dictionary<int, bool> Members 
        //{
        //    get { return members; }
        //    set { members = value; } 
        //}

        public List<int> MembersIds 
        {
            get { return membersIds; }
            set { membersIds = value; } 
        }

        public CollaborationNetwork(int _id, int _communityId)
        {
            id = _id;
            communityId = _communityId;
            membersIds = new List<int>();
            // members = new Dictionary<int, bool>();
        }

        //public void AddMember(int wsId, bool isCompetitive)
        //{
        //    if (!members.ContainsKey(wsId))
        //    {
        //        members.Add(wsId, isCompetitive);
        //    }
        //}

        public void UpdateMemberStatus(int wsId, bool isCompetitive)
        {
            //members[wsId] = isCompetitive;
        }

    }
}
