using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract.DTO;
using ServiceContract.Enums;

namespace ServiceContract
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPersonAdderService
    {
        Task<PersonResponse> AddPerson(PersonAddRequest? request);
       
    }
}
