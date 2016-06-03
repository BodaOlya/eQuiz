using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuiz.Web.Areas.Admin.Models
{
    public interface IQuestion
    {
        int Id { get; set; }
        short? Order { get; set; }
    }
}
