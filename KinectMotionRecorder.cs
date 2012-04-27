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
using Microsoft.Kinect;

namespace KinectMotionRecorder
{
    public class MotionRecorder
    {
        /// <summary>
        /// Refers to the Kinect sensor that will be used
        /// </summary>
        private KinectSensor sensor;
        /// <summary>
        /// Last recorded Motion.
        /// </summary>
        private KinectMotion lastMotion = new KinectMotion();
        /// <summary>
        /// Used by thread to tell when to stop.
        /// </summary>
        private volatile bool isRecording = false;
        /// <summary>
        /// Recording interval in milliseconds.
        /// </summary>
        private int currentRecordingInterval = 100;
        /// <summary>
        /// Lock to ensure consistency in reading/writing lastplayer.
        /// </summary>
        private readonly object player_lock = new object();
        /// <summary>
        /// Lock to ensure consistency in reading/writing isRecording bool.
        /// </summary>
        private readonly object record_bool_lock = new object();
        /// <summary>
        /// Refers to last skeleton detected by Kinect
        /// </summary>
        private Player lastPlayer = new Player();
        /// <summary>
        /// Sets whether or not skeleton should be calculated when new frame is detected.
        /// </summary>
        private Boolean stopped = false;
        /// <summary>
        /// Determines if player has been previously detected
        /// </summary>
        private Boolean skeletonDetected = false;
        /// <summary>
        /// Time recording is started.
        /// </summary>
        private DateTime startTime = DateTime.Now;


        public MotionRecorder()
        {
            try
            {
                //setting up kinect for use
                sensor = KinectSensor.KinectSensors[0];

                sensor.SkeletonFrameReady += SkeletonFrameReadyHandler;
                TransformSmoothParameters tsp = new TransformSmoothParameters();
                sensor.SkeletonStream.Enable(tsp);
                sensor.Start();
            }
            catch (Exception e)
            {
                throw new Exception("Please check to make sure power is supplied to your Kinect.");
            }
        }

        /// <summary>
        /// Starts/stops recording at 500ms interval by default.
        /// </summary>
        public void toggleRecording()
        {
            if (isRecording)
                this.stopRecording();
            else
                this.startRecording(500);
        }

        /// <summary>
        /// Returns a KinectMotion of a single frame.
        /// </summary>
        /// <returns></returns>
        public KinectMotion takeSnapshot()
        {
            KinectMotion km = new KinectMotion();
            km.addKeyframe(lastPlayer.copyPlayer());
            return km;
        }

        /// <summary>
        /// Begins recording from the Kinect. Keyframes made at timeInterval specified. Anything under 150
        /// will likely not work.
        /// </summary>
        /// <param name="timeInterval"></param>
        public void startRecording(int timeInterval)
        {
            // Create the thread object. This does not start the thread.
            currentRecordingInterval = timeInterval;
            System.Threading.Thread recordingThread = new System.Threading.Thread(Record);
            lastMotion = new KinectMotion(timeInterval);
            isRecording = true;
            startTime = DateTime.Now;
            recordingThread.Start();
        }

        /// <summary>
        /// Copies lastplayer to lastMotion keyframe list.
        /// </summary>
        private void Record()
        {
            bool shouldRecord = true;
            while (shouldRecord)
            {
                lock (player_lock)
                {
                    lastMotion.addKeyframe(lastPlayer.copyPlayer());
                }
                System.Threading.Thread.Sleep(currentRecordingInterval);

                lock (record_bool_lock)
                {
                    shouldRecord = isRecording;
                }
            }
        }

        /// <summary>
        /// Stops recording session.
        /// </summary>
        /// <returns></returns>
        public KinectMotion stopRecording()
        {
            lock (record_bool_lock)
            {
                isRecording = false;
            }

            //calculates joint angles for each keyframe in motion
            lastMotion.finalize();

            return lastMotion;
        }

        /// <summary>
        /// Event handler called when a new skeleton frame is detected by the Kinect. Stores X,Y,Z data into
        /// the Player class so that joint angles can be calculated later.
        /// </summary>
        /// <param name="sender">Kinect sensor.</param>
        /// <param name="e">Arguments containing image data.</param>
        private void SkeletonFrameReadyHandler(object sender, SkeletonFrameReadyEventArgs e)
        {
            //if stopped, just return without processing.
            if (stopped)
                return;

            SkeletonFrame sf = e.OpenSkeletonFrame();
            Skeleton[] skeletonArray = new Skeleton[sf.SkeletonArrayLength];
            sf.CopySkeletonDataTo(skeletonArray);

            int i = 0;
            Skeleton skeleton = null;

            //gets the first tracked skeleton; may not work if player
            //leaves then returns to frame
            while ((skeleton == null) && (i < skeletonArray.Length))
            {
                skeleton = skeletonArray[i];
                i++;
            }

            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                //Sets skeleton detected flag.
                if (!skeletonDetected)
                {
                    skeletonDetected = true;
                }

                lock (player_lock)
                {//lock while writing to lastplayer
                    if (lastMotion.poses.Count == 0)
                    {
                        startTime = DateTime.Now;
                    }
                    TimeSpan ts = DateTime.Now - startTime;
                    lastPlayer.time = ts.TotalMilliseconds;

                    //sets x,y,z for each joint tracked
                    foreach (Joint joint in skeleton.Joints)
                    {
                        switch (joint.JointType)
                        {
                            case JointType.AnkleLeft:
                                lastPlayer.lAnkle.x = joint.Position.X;
                                lastPlayer.lAnkle.y = joint.Position.Y;
                                lastPlayer.lAnkle.z = joint.Position.Z;
                                break;
                            case JointType.AnkleRight:
                                lastPlayer.rAnkle.x = joint.Position.X;
                                lastPlayer.rAnkle.y = joint.Position.Y;
                                lastPlayer.rAnkle.z = joint.Position.Z;
                                break;
                            case JointType.ElbowLeft:
                                lastPlayer.lElbow.x = joint.Position.X;
                                lastPlayer.lElbow.y = joint.Position.Y;
                                lastPlayer.lElbow.z = joint.Position.Z;
                                break;
                            case JointType.ElbowRight:
                                lastPlayer.rElbow.x = joint.Position.X;
                                lastPlayer.rElbow.y = joint.Position.Y;
                                lastPlayer.rElbow.z = joint.Position.Z;
                                break;
                            case JointType.HandLeft:
                                lastPlayer.lHand.x = joint.Position.X;
                                lastPlayer.lHand.y = joint.Position.Y;
                                lastPlayer.lHand.z = joint.Position.Z;
                                break;
                            case JointType.HandRight:
                                lastPlayer.rHand.x = joint.Position.X;
                                lastPlayer.rHand.y = joint.Position.Y;
                                lastPlayer.rHand.z = joint.Position.Z;
                                break;
                            case JointType.Head:
                                lastPlayer.head.x = joint.Position.X;
                                lastPlayer.head.y = joint.Position.Y;
                                lastPlayer.head.z = joint.Position.Z;
                                break;
                            case JointType.HipCenter:
                                lastPlayer.cHip.x = joint.Position.X;
                                lastPlayer.cHip.y = joint.Position.Y;
                                lastPlayer.cHip.z = joint.Position.Z;
                                break;
                            case JointType.HipLeft:
                                lastPlayer.lHip.x = joint.Position.X;
                                lastPlayer.lHip.y = joint.Position.Y;
                                lastPlayer.lHip.z = joint.Position.Z;
                                break;
                            case JointType.HipRight:
                                lastPlayer.rHip.x = joint.Position.X;
                                lastPlayer.rHip.y = joint.Position.Y;
                                lastPlayer.rHip.z = joint.Position.Z;
                                break;
                            case JointType.KneeLeft:
                                lastPlayer.lKnee.x = joint.Position.X;
                                lastPlayer.lKnee.y = joint.Position.Y;
                                lastPlayer.lKnee.z = joint.Position.Z;
                                break;
                            case JointType.KneeRight:
                                lastPlayer.rKnee.x = joint.Position.X;
                                lastPlayer.rKnee.y = joint.Position.Y;
                                lastPlayer.rKnee.z = joint.Position.Z;
                                break;
                            case JointType.ShoulderCenter:
                                lastPlayer.neck.x = joint.Position.X;
                                lastPlayer.neck.y = joint.Position.Y;
                                lastPlayer.neck.z = joint.Position.Z;
                                break;
                            case JointType.ShoulderLeft:
                                lastPlayer.lShoulder.x = joint.Position.X;
                                lastPlayer.lShoulder.y = joint.Position.Y;
                                lastPlayer.lShoulder.z = joint.Position.Z;
                                break;
                            case JointType.ShoulderRight:
                                lastPlayer.rShoulder.x = joint.Position.X;
                                lastPlayer.rShoulder.y = joint.Position.Y;
                                lastPlayer.rShoulder.z = joint.Position.Z;
                                break;
                            case JointType.Spine:
                                lastPlayer.spine.x = joint.Position.X;
                                lastPlayer.spine.y = joint.Position.Y;
                                lastPlayer.spine.z = joint.Position.Z;
                                break;
                        }
                    }
                }
            }
        }
    }
}