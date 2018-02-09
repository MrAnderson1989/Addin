using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddInDll
{
    [Description("")]
    public class System
    {
        [Description("{\"HttpMethod\":\"GET\",\"Description\":\"获取全部用户数据\"}")]
        public List<User> Users()
        {
            var lstUser = new List<User>();
            lstUser.Add(new User() { Id = 1, UserName = "Admin", Age = 20, Address = "北京", Remark = "超级管理员" });
            lstUser.Add(new User() { Id = 2, UserName = "张三", Age = 37, Address = "湖南", Remark = "呵呵" });
            lstUser.Add(new User() { Id = 3, UserName = "王五", Age = 32, Address = "广西", Remark = "呵呵" });
            lstUser.Add(new User() { Id = 4, UserName = "韩梅梅", Age = 26, Address = "上海", Remark = "呵呵" });
            lstUser.Add(new User() { Id = 5, UserName = "呵呵", Age = 18, Address = "广东", Remark = "呵呵" });
            return lstUser;
        }

        [Description("{\"HttpMethod\":\"GET\",\"Description\":\"获取指定用户\"}")]
        public User Users(int ID)
        {
            return new User() { Id = 1, UserName = "Admin", Age = 20, Address = "北京", Remark = "超级管理员" };
        }



        [Description("{\"HttpMethod\":\"GET\",\"Description\":\"获取全部用户数据\"}")]
        public string Products()
        {
            return "";
        }

        
    }
}
