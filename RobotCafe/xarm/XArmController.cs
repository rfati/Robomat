using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Common;
using System.ComponentModel;

namespace RobotCafe.xarm
{
    public class XArmController
    {

        public bool IsXArmReady = false;
        private int instance_id = -1;
        private string ip = "";



        public delegate void DeviceDisconnected();
        public event DeviceDisconnected OnDeviceDisconnected;

        public delegate void DeviceConnected();
        public event DeviceConnected OnDeviceConnected;


        private BackgroundWorker xarmBackgroundWorker = null;

        public XArmController(string ip)
        {


            this.ip = ip;
            int ret = -1;

            try
            {
                
                this.instance_id = XArmAPI.create_instance(this.ip, false);
                ret = XArmAPI.switch_xarm(this.instance_id);
                if (ret == 0)
                    ret = XArmAPI.clean_warn();
                if (ret == 0)
                    ret = XArmAPI.clean_error();
                if (ret == 0)
                    ret = XArmAPI.motion_enable(true);
                if (ret == 0)
                    ret = XArmAPI.set_mode(0);
                if (ret == 0)
                    ret = XArmAPI.set_state(0);
                if (ret == 0)
                {
                    this.IsXArmReady = true;
                    xarmBackgroundWorker = new BackgroundWorker();
                    xarmBackgroundWorker.DoWork += new DoWorkEventHandler(XarmBackgroundWorker_DoWork);
                    xarmBackgroundWorker.RunWorkerAsync();
                }
                else
                {
                    this.IsXArmReady = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        private void CloseConnection()
        {
            try
            {
                if (instance_id == -1)
                    return;
                if (xarmBackgroundWorker != null)
                {
                    xarmBackgroundWorker.Dispose();
                }

                XArmAPI.remove_instance(this.instance_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Dispose()
        {
            this.CloseConnection();
        }




        private void XarmBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    if (xarmBackgroundWorker == null)
                    {
                        xarmBackgroundWorker.Dispose();
                        return;
                    }
                    if(this.IsXArmReady)
                    {
                        int is_connected = XArmAPI.connect();
                        if (is_connected == 0)
                        {
                            this.IsXArmReady = true;
                            if (OnDeviceConnected != null)
                                OnDeviceConnected();
                        }
                        else
                        {
                            this.IsXArmReady = false;
                            if (OnDeviceDisconnected != null)
                                OnDeviceDisconnected();
                        }
                    }


                }
                catch (Exception ex)
                {
                    if (OnDeviceDisconnected != null)
                        OnDeviceDisconnected();
                }

                Thread.Sleep(1000);
            }
        }



        public int ClearErrors()
        {
            int ret = -1;
            //ret = XArmAPI.clean_warn();
            //if(this.ret == 0)
            //    ret = XArmAPI.clean_error();
            ret = XArmAPI.clean_error();
            if (ret == 0)
                ret = XArmAPI.motion_enable(true);
            if (ret == 0)
                ret = XArmAPI.set_mode(0);
            if (ret == 0)
                ret = XArmAPI.set_state(0);
            return ret;

        }

        public float[] GetServoAngle()
        {
            float[] angle = { (float)0, (float)0, (float)0, (float)0, (float)0, (float)0, (float)0 };
            int ret = XArmAPI.get_servo_angle(angle);
            if (ret == 0)
            {
                return angle;
            }
            else
            {
                return null;
            }

        }


        public int SetPosition(List<Pos> posList, bool wait = true)
        {
            int ret = -1;
            foreach (var pos in posList)
            {
                pos.Path[0] = pos.Path[0] + (float)2;
                ret = XArmAPI.set_servo_angle(pos.Path, speed: pos.Speed, wait: wait);
                pos.Path[0] = pos.Path[0] - (float)2;
                if (ret != 0)
                {
                    //break;
                    Logger.LogError("XArm Error... set_servo_angle return code: " + ret);
                    Logger.LogInfo("XArm Error... start to clear Errors..!!!");
                    int clearError = this.ClearErrors();
                    if (clearError == 0)
                    {
                        Logger.LogInfo("XArm Error... clear Errors OK--Success");
                        Thread.Sleep(500);
                        pos.Path[0] = pos.Path[0] + (float)2;
                        ret = XArmAPI.set_servo_angle(pos.Path, speed: pos.Speed, wait: wait);
                        pos.Path[0] = pos.Path[0] - (float)2;
                        if (ret != 0)
                        {
                            Logger.LogError("XArm Error next try ... set_servo_angle return code: " + ret);
                            break;
                        }
                    }
                    else
                    {
                        Logger.LogError("XArm Error... clear Errors NOK--Failure!!!!");
                        break;
                    }

                }

            }
            return ret;

        }

        public int SetPosition(List<float[]> angles, float speed = 5, bool wait = true)
        {
            int ret = -1;
            foreach (var angle in angles)
            {
                ret = XArmAPI.set_servo_angle(angle, speed: speed, wait: wait);
                //if (ret != 0)
                //{
                //    //break;
                //    Logger.LogError("XArm Error... set_servo_angle return code: " + ret);
                //    Logger.LogInfo("XArm Error... start to clear Errors..!!!");
                //    int clearError = this.ClearErrors();
                //    if (clearError == 0)
                //    {
                //        Logger.LogInfo("XArm Error... clear Errors OK--Success");
                //        Thread.Sleep(500);
                //        ret = XArmAPI.set_servo_angle(angle, speed: speed, wait: wait);
                //        if (ret != 0)
                //        {
                //            Logger.LogError("XArm Error next try ... set_servo_angle return code: " + ret);
                //            break;
                //        }
                //    }
                //    else
                //    {
                //        Logger.LogError("XArm Error... clear Errors NOK--Failure!!!!");
                //        break;
                //    }

                //}

            }
            return ret;

        }



        public int SetTCPPosition(float[] pos, float speed = 5, bool wait = true)
        {
            return XArmAPI.set_tool_position(pose: pos, speed:speed, wait: wait);

        }
    }
}
