/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace OpenSim.Framework
{
    public interface IAgentData
    {
        UUID AgentID { get; set; }

        OSDMap Pack();
        void Unpack(OSDMap map);
    }

    /// <summary>
    /// Replacement for ChildAgentDataUpdate. Used over RESTComms and LocalComms.
    /// </summary>
    public class AgentPosition : IAgentData
    {
        private UUID m_id;
        public UUID AgentID
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public ulong RegionHandle;
        public uint CircuitCode;
        public UUID SessionID;

        public float Far;
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Center;
        public Vector3 Size;
        public Vector3 AtAxis;
        public Vector3 LeftAxis;
        public Vector3 UpAxis;
        public bool ChangedGrid;

        // This probably shouldn't be here
        public byte[] Throttles;


        public OSDMap Pack()
        {
            OSDMap args = new OSDMap();
            args["message_type"] = OSD.FromString("AgentPosition");

            args["region_handle"] = OSD.FromString(RegionHandle.ToString());
            args["circuit_code"] = OSD.FromString(CircuitCode.ToString());
            args["agent_uuid"] = OSD.FromUUID(AgentID);
            args["session_uuid"] = OSD.FromUUID(SessionID);

            args["position"] = OSD.FromString(Position.ToString());
            args["velocity"] = OSD.FromString(Velocity.ToString());
            args["center"] = OSD.FromString(Center.ToString());
            args["size"] = OSD.FromString(Size.ToString());
            args["at_axis"] = OSD.FromString(AtAxis.ToString());
            args["left_axis"] = OSD.FromString(LeftAxis.ToString());
            args["up_axis"] = OSD.FromString(UpAxis.ToString());

            args["far"] = OSD.FromReal(Far);
            args["changed_grid"] = OSD.FromBoolean(ChangedGrid);

            if ((Throttles != null) && (Throttles.Length > 0))
                args["throttles"] = OSD.FromBinary(Throttles);

            return args;
        }

        public void Unpack(OSDMap args)
        {
            if (args.ContainsKey("region_handle"))
                UInt64.TryParse(args["region_handle"].AsString(), out RegionHandle);

            if (args["circuit_code"] != null)
                UInt32.TryParse((string)args["circuit_code"].AsString(), out CircuitCode);

            if (args["agent_uuid"] != null)
                AgentID = args["agent_uuid"].AsUUID();

            if (args["session_uuid"] != null)
                SessionID = args["session_uuid"].AsUUID();

            if (args["position"] != null)
                Vector3.TryParse(args["position"].AsString(), out Position);

            if (args["velocity"] != null)
                Vector3.TryParse(args["velocity"].AsString(), out Velocity);

            if (args["center"] != null)
                Vector3.TryParse(args["center"].AsString(), out Center);

            if (args["size"] != null)
                Vector3.TryParse(args["size"].AsString(), out Size);

            if (args["at_axis"] != null)
                Vector3.TryParse(args["at_axis"].AsString(), out AtAxis);

            if (args["left_axis"] != null)
                Vector3.TryParse(args["left_axis"].AsString(), out LeftAxis);

            if (args["up_axis"] != null)
                Vector3.TryParse(args["up_axis"].AsString(), out UpAxis);

            if (args["changed_grid"] != null)
                ChangedGrid = args["changed_grid"].AsBoolean();

            if (args["far"] != null)
                Far = (float)(args["far"].AsReal());

            if (args["throttles"] != null)
                Throttles = args["throttles"].AsBinary();
        }
    }

    public class AgentGroupData
    {
        public UUID GroupID;
        public ulong GroupPowers;
        public bool AcceptNotices;

        public AgentGroupData(UUID id, ulong powers, bool notices)
        {
            GroupID = id;
            GroupPowers = powers;
            AcceptNotices = notices;
        }

        public AgentGroupData(OSDMap args)
        {
            UnpackUpdateMessage(args);
        }

        public OSDMap PackUpdateMessage()
        {
            OSDMap groupdata = new OSDMap();
            groupdata["group_id"] = OSD.FromUUID(GroupID);
            groupdata["group_powers"] = OSD.FromString(GroupPowers.ToString());
            groupdata["accept_notices"] = OSD.FromBoolean(AcceptNotices);

            return groupdata;
        }

        public void UnpackUpdateMessage(OSDMap args)
        {
            if (args["group_id"] != null)
                GroupID = args["group_id"].AsUUID();
            if (args["group_powers"] != null)
                UInt64.TryParse((string)args["group_powers"].AsString(), out GroupPowers);
            if (args["accept_notices"] != null)
                AcceptNotices = args["accept_notices"].AsBoolean();
        }
    }

    public class ControllerData
    {
        public UUID ItemID;
        public uint IgnoreControls;
        public uint EventControls;

        public ControllerData(UUID item, uint ignore, uint ev)
        {
            ItemID = item;
            IgnoreControls = ignore;
            EventControls = ev;
        }

        public ControllerData(OSDMap args)
        {
            UnpackUpdateMessage(args);
        }

        public OSDMap PackUpdateMessage()
        {
            OSDMap controldata = new OSDMap();
            controldata["item"] = OSD.FromUUID(ItemID);
            controldata["ignore"] = OSD.FromInteger(IgnoreControls);
            controldata["event"] = OSD.FromInteger(EventControls);

            return controldata;
        }


        public void UnpackUpdateMessage(OSDMap args)
        {
            if (args["item"] != null)
                ItemID = args["item"].AsUUID();
            if (args["ignore"] != null)
                IgnoreControls = (uint)args["ignore"].AsInteger();
            if (args["event"] != null)
                EventControls = (uint)args["event"].AsInteger();
        }
    }

    public class AgentData : IAgentData
    {
        private UUID m_id;
        public UUID AgentID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        public UUID RegionID;
        public uint CircuitCode;
        public UUID SessionID;

        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Center;
        public Vector3 Size;
        public Vector3 AtAxis;
        public Vector3 LeftAxis;
        public Vector3 UpAxis;

        public float Far;
        public float Aspect;
        //public int[] Throttles;
        public byte[] Throttles;

        public uint LocomotionState;
        public Quaternion HeadRotation;
        public Quaternion BodyRotation;
        public uint ControlFlags;
        public float EnergyLevel;
        public Byte GodLevel;
        public float Speed;
        public float DrawDistance;
        public bool AlwaysRun;
        public UUID PreyAgent;
        public Byte AgentAccess;
        public UUID ActiveGroupID;
        public bool SentInitialWearables; 
        //Evil... only for OpenSim...
        public string CallbackURI;

        public AgentGroupData[] Groups;
        public Animation[] Anims;

        public UUID GranterID;

        // Appearance
        public AvatarAppearance Appearance;

        // DEBUG ON
        private static readonly ILog m_log =
                LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);
        // DEBUG OFF

        /*
                public byte[] AgentTextures;
                public byte[] VisualParams;
                public UUID[] Wearables;
                public AvatarAttachment[] Attachments;
        */
        // Scripted
        public ControllerData[] Controllers;

        public virtual OSDMap Pack()
        {
            // DEBUG ON
            //m_log.WarnFormat("[CHILDAGENTDATAUPDATE] Pack data");
            // DEBUG OFF

            OSDMap args = new OSDMap();
            args["message_type"] = OSD.FromString("AgentData");

            args["region_id"] = OSD.FromString(RegionID.ToString());
            args["circuit_code"] = OSD.FromString(CircuitCode.ToString());
            args["agent_uuid"] = OSD.FromUUID(AgentID);
            args["session_uuid"] = OSD.FromUUID(SessionID);

            args["position"] = OSD.FromString(Position.ToString());
            args["velocity"] = OSD.FromString(Velocity.ToString());
            args["center"] = OSD.FromString(Center.ToString());
            args["size"] = OSD.FromString(Size.ToString());
            args["at_axis"] = OSD.FromString(AtAxis.ToString());
            args["left_axis"] = OSD.FromString(LeftAxis.ToString());
            args["up_axis"] = OSD.FromString(UpAxis.ToString());

            args["far"] = OSD.FromReal(Far);
            args["aspect"] = OSD.FromReal(Aspect);

            if ((Throttles != null) && (Throttles.Length > 0))
                args["throttles"] = OSD.FromBinary(Throttles);

            args["locomotion_state"] = OSD.FromString(LocomotionState.ToString());
            args["head_rotation"] = OSD.FromString(HeadRotation.ToString());
            args["body_rotation"] = OSD.FromString(BodyRotation.ToString());
            args["control_flags"] = OSD.FromString(ControlFlags.ToString());

            args["energy_level"] = OSD.FromReal(EnergyLevel);
            args["speed"] = OSD.FromString(Speed.ToString());
            args["god_level"] = OSD.FromString(GodLevel.ToString());
            args["draw_distance"] = OSD.FromReal(DrawDistance);
            args["always_run"] = OSD.FromBoolean(AlwaysRun);
            args["sent_initial_wearables"] = OSD.FromBoolean(SentInitialWearables);
            args["prey_agent"] = OSD.FromUUID(PreyAgent);
            args["agent_access"] = OSD.FromString(AgentAccess.ToString());

            args["active_group_id"] = OSD.FromUUID(ActiveGroupID);

            if ((Groups != null) && (Groups.Length > 0))
            {
                OSDArray groups = new OSDArray(Groups.Length);
                foreach (AgentGroupData agd in Groups)
                    groups.Add(agd.PackUpdateMessage());
                args["groups"] = groups;
            }

            if ((Anims != null) && (Anims.Length > 0))
            {
                OSDArray anims = new OSDArray(Anims.Length);
                foreach (Animation aanim in Anims)
                    anims.Add(aanim.PackUpdateMessage());
                args["animations"] = anims;
            }

            if (Appearance != null)
                args["packed_appearance"] = Appearance.Pack();

            if ((Controllers != null) && (Controllers.Length > 0))
            {
                OSDArray controls = new OSDArray(Controllers.Length);
                foreach (ControllerData ctl in Controllers)
                    controls.Add(ctl.PackUpdateMessage());
                args["controllers"] = controls;
            }

            return args;
        }

        /// <summary>
        /// Deserialization of agent data.
        /// Avoiding reflection makes it painful to write, but that's the price!
        /// </summary>
        /// <param name="hash"></param>
        public virtual void Unpack(OSDMap args)
        {
            // DEBUG ON
            //m_log.WarnFormat("[CHILDAGENTDATAUPDATE] Unpack data");
            // DEBUG OFF

            if (args.ContainsKey("region_id"))
                UUID.TryParse(args["region_id"].AsString(), out RegionID);

            if (args["circuit_code"] != null)
                UInt32.TryParse((string)args["circuit_code"].AsString(), out CircuitCode);

            if (args["agent_uuid"] != null)
                AgentID = args["agent_uuid"].AsUUID();

            if (args["session_uuid"] != null)
                SessionID = args["session_uuid"].AsUUID();

            if (args["position"] != null)
                Vector3.TryParse(args["position"].AsString(), out Position);

            if (args["velocity"] != null)
                Vector3.TryParse(args["velocity"].AsString(), out Velocity);

            if (args["center"] != null)
                Vector3.TryParse(args["center"].AsString(), out Center);

            if (args["size"] != null)
                Vector3.TryParse(args["size"].AsString(), out Size);

            if (args["at_axis"] != null)
                Vector3.TryParse(args["at_axis"].AsString(), out AtAxis);

            if (args["left_axis"] != null)
                Vector3.TryParse(args["left_axis"].AsString(), out AtAxis);

            if (args["up_axis"] != null)
                Vector3.TryParse(args["up_axis"].AsString(), out AtAxis);

            if (args["far"] != null)
                Far = (float)(args["far"].AsReal());

            if (args["aspect"] != null)
                Aspect = (float)args["aspect"].AsReal();

            if (args["throttles"] != null)
                Throttles = args["throttles"].AsBinary();

            if (args["locomotion_state"] != null)
                UInt32.TryParse(args["locomotion_state"].AsString(), out LocomotionState);

            if (args["head_rotation"] != null)
                Quaternion.TryParse(args["head_rotation"].AsString(), out HeadRotation);

            if (args["body_rotation"] != null)
                Quaternion.TryParse(args["body_rotation"].AsString(), out BodyRotation);

            if (args["control_flags"] != null)
                UInt32.TryParse(args["control_flags"].AsString(), out ControlFlags);

            if (args["energy_level"] != null)
                EnergyLevel = (float)(args["energy_level"].AsReal());

            //This IS checked later
            if (args["god_level"] != null)
                Byte.TryParse(args["god_level"].AsString(), out GodLevel);

            if (args["speed"] != null)
                float.TryParse(args["speed"].AsString(), out Speed);
            else
                Speed = 1;

            if (args["draw_distance"] != null)
                float.TryParse(args["draw_distance"].AsString(), out DrawDistance);
            else
                DrawDistance = 0;

            //Reset this to fix movement... since regions are being bad about this
            if (Speed == 0)
                Speed = 1;

            if (args["always_run"] != null)
                AlwaysRun = args["always_run"].AsBoolean();

            if (args["sent_initial_wearables"] != null)
                SentInitialWearables = args["sent_initial_wearables"].AsBoolean();
            else
                SentInitialWearables = false;

            if (args["prey_agent"] != null)
                PreyAgent = args["prey_agent"].AsUUID();

            if (args["callback_uri"] != null)
                CallbackURI = args["callback_uri"].AsString();

            if (args["agent_access"] != null)
                Byte.TryParse(args["agent_access"].AsString(), out AgentAccess);

            if (args["active_group_id"] != null)
                ActiveGroupID = args["active_group_id"].AsUUID();

            if ((args["groups"] != null) && (args["groups"]).Type == OSDType.Array)
            {
                OSDArray groups = (OSDArray)(args["groups"]);
                Groups = new AgentGroupData[groups.Count];
                int i = 0;
                foreach (OSD o in groups)
                {
                    if (o.Type == OSDType.Map)
                    {
                        Groups[i++] = new AgentGroupData((OSDMap)o);
                    }
                }
            }

            if ((args["animations"] != null) && (args["animations"]).Type == OSDType.Array)
            {
                OSDArray anims = (OSDArray)(args["animations"]);
                Anims = new Animation[anims.Count];
                int i = 0;
                foreach (OSD o in anims)
                {
                    if (o.Type == OSDType.Map)
                    {
                        Anims[i++] = new Animation((OSDMap)o);
                    }
                }
            }

            Appearance = new AvatarAppearance(AgentID);

            try
            {
                if (args.ContainsKey("packed_appearance") && (args["packed_appearance"]).Type == OSDType.Map)
                    Appearance = new AvatarAppearance(AgentID, (OSDMap)args["packed_appearance"]);
                // DEBUG ON
                else
                    m_log.WarnFormat("[CHILDAGENTDATAUPDATE] No packed appearance");
                // DEBUG OFF
            }
            catch
            {
            }

            if ((args["controllers"] != null) && (args["controllers"]).Type == OSDType.Array)
            {
                OSDArray controls = (OSDArray)(args["controllers"]);
                Controllers = new ControllerData[controls.Count];
                int i = 0;
                foreach (OSD o in controls)
                {
                    if (o.Type == OSDType.Map)
                    {
                        Controllers[i++] = new ControllerData((OSDMap)o);
                    }
                }
            }
        }

        public AgentData()
        {
        }

        public AgentData(Hashtable hash)
        {
            //UnpackUpdateMessage(hash);
        }

        public void Dump()
        {
            System.Console.WriteLine("------------ AgentData ------------");
            System.Console.WriteLine("UUID: " + AgentID);
            System.Console.WriteLine("Region: " + RegionID);
            System.Console.WriteLine("Position: " + Position);
        }
    }
}
