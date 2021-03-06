﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JlueTaxSystemHuNanBS.Code;
using System.Text;
using JlueTaxSystemHuNanBS.Models;
using System.Web.Mvc;

namespace JlueTaxSystemHuNanBS.Controllers
{
    public class wsbsController : Controller
    {
        private readonly YsbqcSetting set;

        public wsbsController(YsbqcSetting _set)
        {
            set = _set;
        }

        [Route("wsbs/toMainIndex.do")]
        public System.Web.Mvc.ActionResult toMainIndex()
        {
            string questionId = Request.QueryString["questionId"];
            string userquestionId = Request.QueryString["userquestionId"].ToString();
            string companyId = Request.QueryString["companyId"].ToString();
            string classId = Request.QueryString["classid"].ToString();
            string courseId = Request.QueryString["courseid"];
            string userId = Request.QueryString["userid"];
            string Name = Request.QueryString["Name"];

            HttpContext.Session["questionId"]= questionId;
            HttpContext.Session["userquestionId"]= userquestionId;
            HttpContext.Session["companyId"]= companyId;
            HttpContext.Session["classId"]= classId;
            HttpContext.Session["courseId"]= courseId;
            HttpContext.Session["userId"]= userId;
            HttpContext.Session["Name"]= Name;

            JObject jo = new JObject();
            jo["questionId"] = questionId;
            jo["userquestionId"] = userquestionId;
            jo["companyId"] = companyId;
            jo["classId"] = classId;
            jo["courseId"] = courseId;
            jo["userId"] = userId;
            jo["Name"] = Name;

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileFullPath = path + "Session.json";
            StringBuilder str = new StringBuilder();
            str.Append(JsonConvert.SerializeObject(jo));
            StreamWriter sw;
            sw = System.IO.File.CreateText(fileFullPath);
            sw.WriteLine(str.ToString());
            sw.Close();

            Model m = new Model();
            m.Nsrxx = set.getNsrxx();
            return View(m);
        }

        [Route("wsbs/login.do")]
        public JObject login()
        {
            JObject re_json = new JObject();
            string str = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/wsbs/login.json");
            re_json = JsonConvert.DeserializeObject<JObject>(str);
            return re_json;
        }

        [Route("wsbs/ggcx/taxSelect/list_my_info.do")]
        public JObject list_my_info()
        {
            JObject re_json = new JObject();
            string str = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/wsbs/ggcx/taxSelect/list_my_info.json");
            re_json = JsonConvert.DeserializeObject<JObject>(str);
            return re_json;
        }

        [Route("wsbs/ws/taxApplication/list_backlog.do")]
        public System.Web.Mvc.ActionResult list_backlog()
        {
            string str = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/wsbs/ws/taxApplication/list_backlog.json");
            return Content(str, "text/html;charset=utf-8");
        }

        [Route("wsbs/publicService/loadOsCache")]
        public JObject loadOsCache()
        {
            JObject re_json = new JObject();
            string str = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/wsbs/publicService/loadOsCache.json");
            re_json = JsonConvert.DeserializeObject<JObject>(str);
            return re_json;
        }

        [Route("wsbs/publicService/notice/index")]
        public JObject index()
        {
            JObject re_json = new JObject();
            string str = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/wsbs/publicService/notice/index.json");
            re_json = JsonConvert.DeserializeObject<JObject>(str);
            return re_json;
        }

        [Route("wsbs/sb/taxApplication/view_toOnlineDeclare.do")]
        public System.Web.Mvc.ActionResult view_toOnlineDeclare()
        {
            string str = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/wsbs/sb/taxApplication/view_toOnlineDeclare.html");
            return Content(str, "text/html;charset=utf-8");
        }

        [Route("wsbs/sb/taxSelect/declarationInfo/view_DeclarationInfo.do")]
        public System.Web.Mvc.ActionResult view_DeclarationInfo()
        {
            Model m = new Model();
            m.Nsrxx = set.getNsrxx();
            return View(m);
        }

        [Route("wsbs/sb/taxSelect/declarationInfo/list_declarationQuery.do")]
        public JObject list_declarationQuery()
        {
            JObject re_json = new JObject();
            JArray list = new JArray();
            int count = 0;
            string str = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/wsbs/sb/taxSelect/declarationInfo/list_declarationQuery.json");
            re_json = JsonConvert.DeserializeObject<JObject>(str);
            List<GDTXUserYSBQC> listQC = set.getYsbUserYSBQC();
            foreach (GDTXUserYSBQC qc in listQC)
            {
                JObject jo = new JObject();
                JToken reportData = set.getUserYSBQCReportData(qc.Id, qc.BDDM);
                jo["zsxmmc"] = qc.ZSXM;
                jo["sbqx"] = qc.SBQX;
                jo["skssqq"] = qc.SKSSQQ;
                jo["skssqz"] = qc.SKSSQZ;
                jo["sbrq"] = qc.HappenDate;
                jo["pzxh"] = null;
                jo["sbsxDm1"] = null;
                jo["sbsxmc"] = "准期申报";
                jo["nssbrq"] = null;

                if (qc.BDDM == set.BDDM.YbnsrZzs)
                {
                    var query = from d in reportData
                                from dd in d["data"]
                                where dd["name"].ToString() == "wbsbzzsybnsrzb.ybbys"
                                select dd;
                    string ysxssr = query.FirstOrDefault()["value"].ToString();
                    string ynse = query.Where(a => a["idx"].ToString() == "23").FirstOrDefault()["value"].ToString();
                    string ybtse = query.Where(a => a["idx"].ToString() == "33").FirstOrDefault()["value"].ToString();

                    jo["ysxssr"] = ysxssr;
                    jo["ynse"] = ynse;
                    jo["ybtse"] = ybtse;
                    jo["zspmmc"] = "商业(17%、16%、13%)";
                    list.Add(jo);
                    count++;
                }
                else if (qc.BDDM == set.BDDM.Fjs)
                {
                    var query = from d in reportData.SelectToken("[0].data") where d["name"].ToString() == "wbdssbfjsfjssbb.hj" select d;
                    string ysxssr = query.FirstOrDefault()["value"].ToString();
                    query = from d in reportData.SelectToken("[0].data") where d["name"].ToString() == "common.bqynsfehj" select d;
                    string ynse = query.FirstOrDefault()["value"].ToString();
                    query = from d in reportData.SelectToken("[0].data") where d["name"].ToString() == "common.bqybtsehj" select d;
                    string ybtse = query.FirstOrDefault()["value"].ToString();

                    jo["ysxssr"] = ysxssr;
                    jo["ynse"] = ynse;
                    jo["ybtse"] = ybtse;
                    jo["zspmmc"] = "市区（增值税附征）";
                    list.Add(jo);
                    count++;
                }
                else if (qc.BDDM == set.BDDM.Yhs)
                {
                    var query = from d in reportData
                                where d["trs"] != null
                                select d["trs"];
                    foreach (JObject q in query.FirstOrDefault())
                    {
                        JObject joYhs = new JObject(jo);
                        joYhs["ysxssr"] = q.SelectToken("tds.jsje.value");
                        joYhs["ynse"] = q.SelectToken("tds.bqynse.value");
                        joYhs["ybtse"] = q.SelectToken("tds.bqybtse.value");
                        joYhs["zspmmc"] = set.getYhsZspmMc(q.SelectToken("tds.zspmDm.value").ToString());
                        list.Add(joYhs);
                        count++;
                    }
                }
                else if (qc.BDDM == set.BDDM.Qysds)
                {
                    var query = from d in reportData
                                from dd in d["data"]
                                where dd["name"].ToString() == "wbsbqysdsa2018a200000.lrzelj"
                                select dd;
                    string ysxssr = query.FirstOrDefault()["value"].ToString();

                    query = from d in reportData
                            from dd in d["data"]
                            where dd["name"].ToString() == "wbsbqysdsa2018a200000.ynsdselj"
                            select dd;
                    string ynse = query.FirstOrDefault()["value"].ToString();

                    query = from d in reportData
                            from dd in d["data"]
                            where dd["name"].ToString() == "wbsbqysdsa2018a200000.ybtsdselj"
                            select dd;
                    string ybtse = query.FirstOrDefault()["value"].ToString();

                    jo["ysxssr"] = ysxssr;
                    jo["ynse"] = ynse;
                    jo["ybtse"] = ybtse;
                    jo["zspmmc"] = "企业所得税月季度A表";
                    list.Add(jo);
                    count++;
                }

            }
            re_json["list"] = list;
            re_json["dto"]["count"] = count;
            return re_json;
        }

        [Route("wsbs/message/publicService/list_message.do")]
        public JObject list_message()
        {
            JObject re_json = new JObject();
            string str = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/wsbs/message/publicService/list_message.json");
            re_json = JsonConvert.DeserializeObject<JObject>(str);
            return re_json;
        }

        [Route("wsbs/myInfo/toUserInfo.do")]
        public System.Web.Mvc.ActionResult toUserInfo()
        {
            return View(set.functionNotOpen);
        }

        [Route("wsbs/sb/menusearch/view.do")]
        public System.Web.Mvc.ActionResult menusearch()
        {
            return View(set.functionNotOpen);
        }

    }
}