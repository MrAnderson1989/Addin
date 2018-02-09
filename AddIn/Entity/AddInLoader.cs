using AddIn.Interface;
using AddIn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AddIn.Entity
{
    public class AddInLoader : MarshalByRefObject, IAddInLoader
    {

        public override object InitializeLifetimeService()
        {
            //Remoting对象 无限生存期
            return null;
        }

        public AddInLoader()
        {
            Controllers = new List<IController>();
            ControllerDic = new Dictionary<string, Type>();
            ActionDic = new Dictionary<string, MethodInfo>();
            InstanceDic = new Dictionary<Type, object>();
            DllPathDic = new List<string>();
        }

        private static List<IController> Controllers { get; set; }

        public object Execute(string Controller, string Action, object[] Parameters)
        {
            Controller = Controller.ToLower();
            Action = Action.ToLower();
            object instance = GetControllerInstance(Controller);
            return RunAction(instance, Action, Parameters);
        }

        public bool LoadAssembly(string Path)
        {
            try
            {
                if (!DllPathDic.Contains(Path))
                {
                    Assembly assembly = Assembly.LoadFrom(Path);
                    List<Type> types = assembly.GetTypes().ToList();
                    //合并去重字典缓存
                    ControllerDic = ControllerDic == null ? new Dictionary<string, Type>() : ControllerDic;
                    ControllerDic = ControllerDic.Union(types.Where(w => w.GetCustomAttribute(typeof(DescriptionAttribute), false) != null && ((DescriptionAttribute)w.GetCustomAttribute(typeof(DescriptionAttribute), false)).Description == "Controller").ToDictionary(type => type.Name.ToLower(), type => type)).ToDictionary(t => t.Key, t => t.Value);
                    foreach (var t in types)
                    {
                        Attribute attr = t.GetCustomAttribute(typeof(DescriptionAttribute), false);
                        if (attr != null && ((DescriptionAttribute)attr).Description == "Controller")
                        {
                            foreach (var m in t.GetMethods())
                            {

                                if (!ActionDic.ContainsKey(m.Name.ToLower()))
                                {
                                    ActionDic.Add(m.Name.ToLower(), m);
                                }

                            }
                        }
                        //ActionDic = ActionDic.Union(t.GetMethods().ToDictionary(m => m.Name.ToLower(), m => m)).ToDictionary(m => m.Key, m => m.Value);
                    }
                    //初始化插件数据结构
                    InitControllersData();
                    DllPathDic.Add(Path);
                    return true;
                }
                else
                {
                    throw new Exception("程序集重复加载！");
                }
            }
            catch (Exception e)
            {
                Log.Write(e.ToString());
                throw e;
            }
        }

        public List<IController> GetControllers()
        {
            return Controllers;
        }

        #region 私有成员

        private static Dictionary<string, Type> ControllerDic { get; set; }
        private static Dictionary<string, MethodInfo> ActionDic { get; set; }
        private static Dictionary<Type, object> InstanceDic { get; set; }

        private List<string> DllPathDic { get; set; }

        private object GetControllerInstance(string Controller)
        {

            if (ControllerDic.ContainsKey(Controller))
            {
                object obj = null;
                if (InstanceDic.ContainsKey(ControllerDic[Controller]))
                {
                    obj = InstanceDic[ControllerDic[Controller]];
                }
                else
                {
                    obj = Activator.CreateInstance(ControllerDic[Controller]);
                    InstanceDic.Add(ControllerDic[Controller], obj);
                }
                return obj;
            }
            else
            {
                throw new Exception("Controller not found!");
            }
        }

        private object RunAction(object Instance, string Action, object[] Parameters)
        {

            if (ActionDic.ContainsKey(Action))
            {
                if (Parameters != null)
                {
                    Parameters = ConvertParameterType(Parameters, ActionDic[Action]);
                }
                return ActionDic[Action].Invoke(Instance, Parameters);
            }
            else
            {
                throw new Exception("Action not found");
            }
        }

        private object[] ConvertParameterType(object[] Parameters, MethodInfo ActionInfo)
        {
            try
            {
                List<object> result = new List<object>();
                ParameterInfo[] pis = ActionInfo.GetParameters();
                if (pis.Length == Parameters.Length)
                {
                    for (int i = 0; i < pis.Length; i++)
                    {
                        result.Add(ConvertType(Parameters[i], pis[i].ParameterType));
                    }
                    return result.ToArray();
                }
                else
                {
                    throw new Exception("参数数量不匹配！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private object ConvertType(object obj, Type type)
        {
            try
            {
                return Convert.ChangeType(obj, type);
            }
            catch
            {
                throw new Exception("参数类型不匹配！");
            }
        }

        private void InitControllersData()
        {
            foreach (var c in ControllerDic)
            {
                if (!c.Key.Contains("<") && !c.Key.Contains(">"))
                {
                    AddInController controller = new AddInController();
                    controller.ControllerName = c.Value.Name;
                    foreach (var m in c.Value.GetMethods())
                    {
                        if (m.Name != "ToString" && m.Name != "Equals" && m.Name != "GetHashCode" && m.Name != "GetType")
                        {
                            AddInAction action = new AddInAction();
                            action.ActionName = m.Name;
                            Attribute attr = m.GetCustomAttribute(typeof(DescriptionAttribute), false);
                            if (attr != null)
                            {
                                if (((DescriptionAttribute)attr).Description.Contains("|"))
                                {
                                    string[] description = ((DescriptionAttribute)attr).Description.Split('|');
                                    action.HttpMethod = description[0];
                                    action.Description = description[1];
                                }
                            }
                            foreach (var p in m.GetParameters())
                            {
                                AddInParameter parameter = new AddInParameter();
                                parameter.ParameterName = p.Name;
                                parameter.ParameterType = p.ParameterType;
                                action.Parameters.Add(parameter);
                            }
                            action.ReturnType = m.ReturnType;
                            controller.Actions.Add(action);
                        }
                    }
                    Controllers.Add(controller);
                }
            }
        }

        #endregion


    }
}
