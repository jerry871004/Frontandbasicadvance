using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Library.Models
{
    public class BookRecord
    {
        [DisplayName("借閱日期")]
        public string LendDate { get; set; }

        [DisplayName("借閱人編號")]
        public string KeeperId { get; set; }

        [DisplayName("英文姓名")]
        public string KeeperEname { get; set; }

        [DisplayName("中文姓名")]
        public string KeeperCname { get; set; }
    }
}