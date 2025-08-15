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
    public interface IPersonUpdaterService
    {
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request);
    }
}
