/* * * * * * * * * * * * * *
 *  This file is part of the KinectMotionRecorder library.
 *
 *  KinectMotionRecorder is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  KinectMotionRecorder is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with KinectMotionRecorder.  If not, see <http://www.gnu.org/licenses/>.
 * * * * * * * * * * * * * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace KinectMotionRecorder
{
    public class JointAngles
    {
        public string name;
        public float roll = 0;
        public float pitch = 0;
        public float yaw = 0;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float fConfidence = 0;

        public JointAngles(string n)
        {
            name = n;
        }

        /// <summary>
        /// Makes new JointAngles from current values.
        /// </summary>
        /// <returns></returns>
        public JointAngles copyJointAngles()
        {
            JointAngles copy = new JointAngles(this.name);
            copy.roll = this.roll;
            copy.pitch = this.pitch;
            copy.yaw = this.yaw;
            copy.x = this.x;
            copy.y = this.y;
            copy.z = this.z;
            copy.fConfidence = this.fConfidence;
            return copy;
        }

        /// <summary>
        /// Copies current values to other joint angle.
        /// </summary>
        /// <param name="copy"></param>
        public void copyTo(JointAngles copy)
        {
            copy.name = this.name;
            copy.roll = this.roll;
            copy.pitch = this.pitch;
            copy.yaw = this.yaw;
            copy.x = this.x;
            copy.y = this.y;
            copy.z = this.z;
            copy.fConfidence = this.fConfidence;
        }
    }

    public class Player
    {
        public double time = 0.0;
        public List<JointAngles> jointList = new List<JointAngles>();

        public JointAngles head = new JointAngles("Head");
        public JointAngles neck = new JointAngles("Neck");
        public JointAngles spine = new JointAngles("Spine");
        public JointAngles lShoulder = new JointAngles("Left Shoulder");
        public JointAngles rShoulder = new JointAngles("Right Shoulder");
        public JointAngles rElbow = new JointAngles("Right Elbow");
        public JointAngles lElbow = new JointAngles("Left Elbow");
        public JointAngles rHand = new JointAngles("Right Hand");
        public JointAngles lHand = new JointAngles("Left Hand");
        public JointAngles rHip = new JointAngles("Right Hip");
        public JointAngles lHip = new JointAngles("Left Hip");
        public JointAngles cHip = new JointAngles("Center Hip");
        public JointAngles lKnee = new JointAngles("Left Knee");
        public JointAngles rKnee = new JointAngles("Right Knee");
        public JointAngles pelvis = new JointAngles("Pelvis");
        public JointAngles rAnkle = new JointAngles("Right Ankle");
        public JointAngles lAnkle = new JointAngles("Left Ankle");

        public Player()
        {
            jointList.Add(head);
            jointList.Add(neck);
            jointList.Add(spine);
            jointList.Add(lShoulder);
            jointList.Add(rShoulder);
            jointList.Add(rElbow);
            jointList.Add(lElbow);
            jointList.Add(rHand);
            jointList.Add(lHand);
            jointList.Add(rHip);
            jointList.Add(lHip);
            jointList.Add(cHip);
            jointList.Add(lKnee);
            jointList.Add(rKnee);
            jointList.Add(pelvis);
            jointList.Add(rAnkle);
            jointList.Add(lAnkle);
        }

        public Player copyPlayer()
        {
            Player newPlayer = new Player();
            this.copyTo(newPlayer);

            return newPlayer;
        }

        public void copyTo(Player copy)
        {
            copy.time = this.time;
            this.head.copyTo(copy.head);
            this.neck.copyTo(copy.neck);
            this.spine.copyTo(copy.spine);
            this.lShoulder.copyTo(copy.lShoulder);
            this.rShoulder.copyTo(copy.rShoulder);
            this.rElbow.copyTo(copy.rElbow);
            this.lElbow.copyTo(copy.lElbow);
            this.rHand.copyTo(copy.rHand);
            this.lHand.copyTo(copy.lHand);
            this.rHip.copyTo(copy.rHip);
            this.lHip.copyTo(copy.lHip);
            this.cHip.copyTo(copy.cHip);
            this.lKnee.copyTo(copy.lKnee);
            this.rKnee.copyTo(copy.rKnee);
            this.pelvis.copyTo(copy.pelvis);
            this.rAnkle.copyTo(copy.rAnkle);
            this.lAnkle.copyTo(copy.lAnkle);
        }
    }

    public class KinectMotion
    {
        /// <summary>
        /// original time recorded
        /// </summary>
        private float Length = 0;
        public float length
        {
            get { return Length; }
        }
        /// <summary>
        /// number of keyframes
        /// </summary>
        private int Keyframes = 0;
        public int keyframes
        {
            get { return Keyframes; }
        }
        /// <summary>
        /// actual stored poses
        /// </summary>
        private List<Player> Poses = new List<Player>();
        public List<Player> poses
        {
            get { return Poses; }
        }
        /// <summary>
        /// Intended time interval between keyframes. Not necessarily
        /// actual time between snapshots.
        /// </summary>
        private float TimeInterval;
        public float timing
        {
            get { return TimeInterval; }
        }

        /// <summary>
        /// Total time of recorded motion.
        /// </summary>
        private float Seconds;
        public float seconds
        {
            get { return Seconds; }
        }

        /// <summary>
        /// Adds pose to list of poses.
        /// </summary>
        /// <param name="pose"></param>
        public void addKeyframe(Player pose)
        {
            Keyframes++;
            Poses.Add(pose);
        }

        public KinectMotion() : this(200) { }

        public KinectMotion(int interval) : this(interval, 0, 0)
        {
            TimeInterval = interval;
            Seconds = 0;
        }

        /// <summary>
        /// Used when reading motion from XML.
        /// </summary>
        /// <param name="interval">Intended interval between poses.</param>
        /// <param name="seconds">Total time from start to finish of motion.</param>
        /// <param name="keys">Number of poses.</param>
        private KinectMotion(int interval, float seconds, int keys)
        {
            TimeInterval = interval;
            Seconds = seconds;
            Keyframes = keys;
        }

        /// <summary>
        /// Sets time of everything and calculates the joint angles. Important to
        /// call this when done recording.
        /// </summary>
        public void finalize()
        {
            for (int i = 0; i < this.poses.Count; i++)
            {
                calculateAngles(this.poses[i]);
            }

            Seconds = (float)(this.poses[poses.Count-1].time / 1000);
        }

        //XML Stuff Below follows format
        /******************
         * Returns new KinectMotion by reading XML tree of the form:
         * <root>
         *  <Data Interval="200" Length="3.397" Keyframes="18">
         *      <Frame Time="0">
         *          <JointData>
         *          <Name>Head</Name>
         *          <Position>
         *              <X>0.06443969</X>
         *              <Y>0.8475291</Y>
         *              <Z>1.98863149</Z>
         *          </Position>
         *          <Angles>
         *              <Roll>0</Roll>
         *              <Yaw>0</Yaw>
         *              <Pitch>19.868578</Pitch>
         *          </Angles>
         *          </JointData>
         *          ...
         *      </Frame>
         *      ...
         *  <Data>
         * </root>
         * ****************/
        //public System.Xml.Schema.XmlSchema GetSchema()
        //{
        //    return null;
        //}

        /// <summary>
        /// Returns XML that represents entire movement in format outlined above.
        /// </summary>
        /// <returns></returns>
        public XElement getXML()
        {
            XElement motionXML = new XElement("Data");
            motionXML.SetAttributeValue("Interval", TimeInterval.ToString());
            motionXML.SetAttributeValue("Length", Seconds.ToString());
            motionXML.SetAttributeValue("Keyframes", Keyframes.ToString());

            foreach (Player player in this.poses)
            {
                XElement frame = new XElement("Frame");
                frame.SetAttributeValue("Time", player.time.ToString());

                foreach (JointAngles joint in player.jointList){
                    XElement tempJoint = new XElement("JointData",
                        new XElement("Name",joint.name),
                        new XElement("Position",
                            new XElement("X", joint.x),
                            new XElement("Y", joint.y),
                            new XElement("Z", joint.z)
                            ),
                        new XElement("Angles",
                            new XElement("Roll", joint.roll),
                            new XElement("Yaw", joint.yaw),
                            new XElement("Pitch", joint.pitch)
                            )
                    );

                    frame.Add(tempJoint);
                }

                motionXML.Add(frame);
            }

            return motionXML;
        }
        
        /// <summary>
        /// Reads from XML format outlined above.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static KinectMotion readFromXML(XElement root)
        {
            List<XElement> dataList = root.Descendants("Data").ToList<XElement>();
            if(dataList == null)
                return null;
            XElement dataElement = dataList[0];

            KinectMotion motion = new KinectMotion(int.Parse(dataElement.Attribute("Interval").Value),
                float.Parse(dataElement.Attribute("Length").Value), int.Parse(dataElement.Attribute("Keyframes").Value));

            IEnumerable<XElement> frames = dataElement.Descendants("Frame");

            Player newFrame;
            foreach (XElement frame in frames)
            {
                newFrame = new Player();

                foreach (XElement joint in frame.Descendants("JointData")){
                    JointAngles jangles = KinectMotion.parseJoint(joint);

                    XElement nameNode = joint.Descendants("Name").ToList<XElement>()[0];

                    switch (nameNode.Value){
                        case "Head":
                            jangles.name = "Head";
                            newFrame.head = jangles;
                            break;
                        case "Neck":
                            jangles.name = "Neck";
                            newFrame.neck = jangles;
                            break;
                        case "Spine":
                            jangles.name = "Spine";
                            newFrame.spine = jangles;
                            break;
                        case "Left Shoulder":
                            jangles.name = "Left Shoulder";
                            newFrame.lShoulder = jangles;
                            break;
                        case "Right Shoulder":
                            jangles.name = "Right Shoulder";
                            newFrame.rShoulder = jangles;
                            break;
                        case "Left Elbow":
                            jangles.name = "Left Elbow";
                            newFrame.lElbow = jangles;
                            break;
                        case "Right Elbow":
                            jangles.name = "Right Elbow";
                            newFrame.rElbow = jangles;
                            break;
                        case "Right Hand":
                            jangles.name = "Right Hand";
                            newFrame.rHand = jangles;
                            break;
                        case "Left Hand":
                            jangles.name = "Left Hand";
                            newFrame.lHand = jangles;
                            break;
                        case "Right Hip":
                            jangles.name = "Right Hip";
                            newFrame.rHip = jangles;
                            break;
                        case "Left Hip":
                            jangles.name = "Left Hip";
                            newFrame.lHip = jangles;
                            break;
                        case "Center Hip":
                            jangles.name = "Center Hip";
                            newFrame.cHip = jangles;
                            break;
                        case "Left Knee":
                            jangles.name = "Left Knee";
                            newFrame.lKnee = jangles;
                            break;
                        case "Right Knee":
                            jangles.name = "Right Knee";
                            newFrame.rKnee = jangles;
                            break;
                        case "Pelvis":
                            jangles.name = "Pelvis";
                            newFrame.pelvis = jangles;
                            break;
                        case "Right Ankle":
                            jangles.name = "Right Ankle";
                            newFrame.rAnkle = jangles;
                            break;
                        case "Left Ankle":
                            jangles.name = "Left Ankle";
                            newFrame.lAnkle = jangles;
                            break;
                    }
                }

                //have to copy to keep list references?
                motion.addKeyframe(newFrame.copyPlayer());
            }

            return motion;
        }

        /// <summary>
        /// Reads in joint data specified above.
        /// </summary>
        /// <param name="joint"></param>
        /// <returns></returns>
        public static JointAngles parseJoint(XElement joint)
        {
            JointAngles jointAngles = new JointAngles("Temp");

            foreach (XElement element in joint.Descendants())
            {
                if (element.Name == "Position")
                {
                    foreach (XElement position in element.Descendants())
                    {
                        if (position.Name == "X")
                            jointAngles.x = float.Parse(position.Value);
                        else if (position.Name == "Y")
                            jointAngles.y = float.Parse(position.Value);
                        else if (position.Name == "Z")
                            jointAngles.z = float.Parse(position.Value);
                    }
                }
                else if (element.Name == "Angles")
                {
                    foreach (XElement position in element.Descendants())
                    {
                        if (position.Name == "Roll")
                            jointAngles.roll = float.Parse(position.Value);
                        else if (position.Name == "Yaw")
                            jointAngles.yaw = float.Parse(position.Value);
                        else if (position.Name == "Pitch")
                            jointAngles.pitch = float.Parse(position.Value);
                    }
                }
            }

            return jointAngles;
        }

        /*
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Data");
            writer.WriteAttributeString("Interval", TimeInterval.ToString());
            writer.WriteAttributeString("Time", Seconds.ToString());
            writer.WriteAttributeString("Keyframes", Keyframes.ToString());

            writer.WriteStartElement("Frame");
            foreach (Player player in this.poses){
                writer.WriteElementString("Time", player.time.ToString());

                foreach (JointAngles joint in player.jointList)
                {
                    writer.WriteStartElement("JointData");
                    writer.WriteElementString("Name", joint.name);

                    writer.WriteStartElement("Position");
                    writer.WriteElementString("X", joint.x.ToString());
                    writer.WriteElementString("Y", joint.y.ToString());
                    writer.WriteElementString("Z", joint.z.ToString());
                    writer.WriteEndElement();

                    writer.WriteStartElement("Angle");
                    writer.WriteElementString("Roll", joint.roll.ToString());
                    writer.WriteElementString("Yaw", joint.yaw.ToString());
                    writer.WriteElementString("Pitch", joint.pitch.ToString());
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Seconds = float.Parse(reader.GetAttribute("Seconds"));
            Keyframes = int.Parse(reader.GetAttribute("Keyframes"));
            TimeInterval = int.Parse(reader.GetAttribute("Interval"));

        }*/

        /// <summary>
        /// Calculates all of the joint angles for the current Player stored in lastPlayer
        /// and also stores them in lastPlayer to be used later.
        /// </summary>
        public static void calculateAngles(Player player)
        {
            List<float> neck_vec = getDirectionVector(player.spine, player.neck);
            List<float> head_vec = getDirectionVector(player.neck, player.head);

            float headPitch = getAngleBetweenVecs(neck_vec, head_vec);
            if (neck_vec[2] < head_vec[2]) headPitch = headPitch * -1.0f;

            player.head.fConfidence = 1.0f;
            player.head.roll = 0.0f;
            player.head.pitch = headPitch;
            player.head.yaw = 0.0f;



            List<float> rShoulder_vec = getDirectionVector(player.lShoulder,
                                                                    player.rShoulder);
            List<float> lShoulder_vec = getDirectionVector(player.rShoulder,
                                                                    player.lShoulder);
            List<float> rUpperArm_vec = getDirectionVector(player.rShoulder,
                                                                    player.rElbow);
            List<float> lUpperArm_vec = getDirectionVector(player.lShoulder,
                                                                    player.lElbow);
            List<float> center_vec = getDirectionVector(player.neck,
                                                                    player.spine);

            float rShoulderRoll = getAngleBetweenVecs(rShoulder_vec, rUpperArm_vec) - 90.0f;
            float lShoulderRoll = Math.Abs(getAngleBetweenVecs(lShoulder_vec, lUpperArm_vec) - 90.0f);

            float rShoulderPitch = (-1.0f * getAngleBetweenVecs(center_vec, rUpperArm_vec)) + 90.0f;
            float lShoulderPitch = (-1.0f * getAngleBetweenVecs(center_vec, lUpperArm_vec)) + 90.0f;

            player.rShoulder.fConfidence = 1.0f;
            player.rShoulder.roll = rShoulderRoll;
            player.rShoulder.pitch = rShoulderPitch;
            player.rShoulder.yaw = 0.0f;

            player.lShoulder.fConfidence = 1.0f;
            player.lShoulder.roll = lShoulderRoll;
            player.lShoulder.pitch = lShoulderPitch;
            player.lShoulder.yaw = 0.0f;



            List<float> rLowerArm_vec = getDirectionVector(player.rElbow,
                                                                    player.rHand);
            List<float> lLowerArm_vec = getDirectionVector(player.lElbow,
                                                                    player.lHand);
            List<float> rShCenter_vec = getDirectionVector(player.rShoulder,
                                                                    player.neck);
            List<float> lShCenter_vec = getDirectionVector(player.lShoulder,
                                                                    player.neck);


            float rElbowRoll = getAngleBetweenVecs(rUpperArm_vec, rLowerArm_vec);
            float lElbowRoll = getAngleBetweenVecs(lUpperArm_vec, lLowerArm_vec) * -1.0f;

            List<float> dVecRightShoulder = getVectorProduct(rShCenter_vec, rUpperArm_vec);
            List<float> dVecLeftShoulder = getVectorProduct(lShCenter_vec, lUpperArm_vec);

            float rElbowYaw = getAngleBetweenVecs(rLowerArm_vec, dVecRightShoulder) - 90.0f;
            float lElbowYaw = getAngleBetweenVecs(lLowerArm_vec, dVecLeftShoulder) - 90.0f;

            player.rElbow.fConfidence = 1.0f;
            player.rElbow.roll = rElbowRoll;
            player.rElbow.pitch = 0.0f;
            player.rElbow.yaw = rElbowYaw;

            player.lElbow.fConfidence = 1.0f;
            player.lElbow.roll = lElbowRoll;
            player.lElbow.pitch = 0.0f;
            player.lElbow.yaw = lElbowYaw;



            List<float> cHip_vec = getDirectionVector(player.cHip,
                                                                    player.spine);
            List<float> rUpperLeg_vec = getDirectionVector(player.rHip,
                                                                    player.rKnee);
            List<float> lUpperLeg_vec = getDirectionVector(player.lHip,
                                                                    player.lKnee);

            float HipYawPitch = (getAngleBetweenVecs(cHip_vec, center_vec) - 150.0f) * -1.0f;

            float rHipPitch = getAngleBetweenVecs(center_vec, rUpperLeg_vec) * -1.0f;
            float lHipPitch = getAngleBetweenVecs(center_vec, lUpperLeg_vec) * -1.0f;



            if (center_vec[2] < rUpperLeg_vec[2]) rHipPitch = rHipPitch * -1.0f;
            if (center_vec[2] < lUpperLeg_vec[2]) lHipPitch = lHipPitch * -1.0f;

            float rHipRoll = getRoll(rUpperLeg_vec);
            float lHipRoll = getRoll(lUpperLeg_vec);

            player.rHip.fConfidence = 1.0f;
            player.rHip.roll = rHipRoll;
            player.rHip.pitch = rHipPitch;
            player.rHip.yaw = HipYawPitch;

            player.lHip.fConfidence = 1.0f;
            player.lHip.roll = lHipRoll;
            player.lHip.pitch = lHipPitch;
            player.lHip.yaw = HipYawPitch;



            List<float> rLowerLeg_vec = getDirectionVector(player.rKnee,
                                                                    player.rAnkle);
            List<float> lLowerLeg_vec = getDirectionVector(player.lKnee,
                                                                    player.lAnkle);

            float rKneePitch = getAngleBetweenVecs(rUpperLeg_vec, rLowerLeg_vec);
            float lKneePitch = getAngleBetweenVecs(lUpperLeg_vec, lLowerLeg_vec);

            player.rKnee.fConfidence = 1.0f;
            player.rKnee.roll = 0.0f;
            player.rKnee.pitch = rKneePitch;
            player.rKnee.yaw = 0.0f;

            player.lKnee.fConfidence = 1.0f;
            player.lKnee.roll = 0.0f;
            player.lKnee.pitch = lKneePitch;
            player.lKnee.yaw = 0.0f;

            List<float> Sh_vec = getDirectionVector(player.rShoulder,
                                                            player.lShoulder);

            List<float> y_axis = new List<float>();
            y_axis.Add(0.0f);
            y_axis.Add(1.0f);
            y_axis.Add(0.0f);

            List<float> z_axis = new List<float>();
            z_axis.Add(0.0f);
            z_axis.Add(0.0f);
            z_axis.Add(1.0f);

            player.pelvis.fConfidence = 1.0f;
            player.pelvis.x = 0.0f;
            player.pelvis.y = getAngleBetweenVecs(center_vec, y_axis); // PITCH
            player.pelvis.z = getAngleBetweenVecs(Sh_vec, z_axis); // YAW

            player.rHand.fConfidence = 1.0f;
            player.rHand.roll = 0.0f;
            player.rHand.pitch = 0.0f;
            player.rHand.yaw = 0.0f;

            player.lHand.fConfidence = 1.0f;
            player.lHand.roll = 0.0f;
            player.lHand.pitch = 0.0f;
            player.lHand.yaw = 0.0f;

            player.rAnkle.fConfidence = 1.0f;
            player.rAnkle.roll = 0.0f;
            player.rAnkle.pitch = degreeToRadian(-20.0f);
            player.rAnkle.yaw = 0.0f;

            player.lAnkle.fConfidence = 1.0f;
            player.lAnkle.roll = 0.0f;
            player.lAnkle.pitch = degreeToRadian(-20.0f);
            player.lAnkle.yaw = 0.0f;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="deg">Degree to be converted.</param>
        /// <returns>Radian equivalent of degree input.</returns>
        private static float degreeToRadian(double deg)
        {
            return (float)(deg * Math.PI / 180);
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="rad">Radian to be converted.</param>
        /// <returns>Degree equivalent of radian.</returns>
        private static float radianToDegree(double rad)
        {
            return (float)(rad * 180 / Math.PI);
        }

        /// <summary>
        /// Calculates vector between two points in space.
        /// </summary>
        /// <param name="joint1">First point in space.</param>
        /// <param name="joint2">Second point in space.</param>
        /// <returns></returns>
        private static List<float> getDirectionVector(JointAngles joint1, JointAngles joint2)
        {
            List<float> d_vec = new List<float>();
            d_vec.Add(joint2.x - joint1.x);
            d_vec.Add(joint2.y - joint1.y);
            d_vec.Add(joint2.z - joint1.z);

            return d_vec;
        }

        /// <summary>
        /// Calculates angle between two vectors.
        /// </summary>
        /// <param name="vec1">First vector.</param>
        /// <param name="vec2">Second vector.</param>
        /// <returns></returns>
        private static float getAngleBetweenVecs(List<float> vec1, List<float> vec2)
        {
            float x = (vec1[0] * vec2[0]) + (vec1[1] * vec2[1]) + (vec1[2] * vec2[2]);
            float y = (float)((Math.Sqrt((vec1[0] * vec1[0]) + (vec1[1] * vec1[1]) + (vec1[2] * vec1[2]))) * (Math.Sqrt((vec2[0] * vec2[0]) + (vec2[1] * vec2[1]) + (vec2[2] * vec2[2]))));
            return radianToDegree(Math.Acos(x / y));
        }

        /// <summary>
        /// Calculates cross product between vectors.
        /// </summary>
        /// <param name="vec1">First vector.</param>
        /// <param name="vec2">Second vector.</param>
        /// <returns></returns>
        private static List<float> getVectorProduct(List<float> vec1, List<float> vec2)
        {
            List<float> result = new List<float>();
            result.Add((vec1[1] * vec2[2]) - (vec2[1] * vec1[2]));
            result.Add((vec1[2] * vec2[0]) - (vec2[2] * vec1[0]));
            result.Add((vec1[0] * vec2[1]) - (vec2[0] * vec1[1]));

            return result;
        }

        /// <summary>
        /// Calculates roll angle.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        private static float getRoll(List<float> vec)
        {
            float HYP = (float)Math.Sqrt((vec[0] * vec[0]) + (vec[1] * vec[1]) + (vec[2] * vec[2]));
            float GK = (float)Math.Sqrt((vec[0] * vec[0]));

            int quadrant = solveQuadrant(vec);
            float result = 0.0f;

            if (quadrant == 1 || quadrant == 2 || quadrant == 3 || quadrant == 4)
            {
                result = radianToDegree(Math.Asin(GK / HYP));
            }
            else
            {
                result = -radianToDegree(Math.Asin(GK / HYP));
            }
            return result;
        }

        /// <summary>
        /// Identifies which quadrant(octant) the vector is in.
        /// </summary>
        /// <param name="directed_vector"></param>
        /// <returns></returns>
        private static int solveQuadrant(List<float> directed_vector)
        {
            if (directed_vector[1] > 0.0)
            {
                if (directed_vector[2] < 0.0)
                {
                    if (directed_vector[0] < 0.0) { return 1; }
                    else { return 5; }
                }
                else
                {
                    if (directed_vector[0] < 0.0) { return 2; }
                    else { return 6; }
                }
            }
            else
            {
                if (directed_vector[2] < 0.0)
                {
                    if (directed_vector[0] < 0.0) { return 4; }
                    else { return 8; }
                }
                else
                {
                    if (directed_vector[0] < 0.0) { return 3; }
                    else { return 7; }
                }
            }
        }
    }
}
