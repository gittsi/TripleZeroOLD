using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TripleZeroApi.Repository;
using SWGoH;
using TripleZeroApi.Models;
using System.Net;
using TripleZeroApi.Infrastructure.DI;

namespace TripleZeroApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Player")]
    public class PlayerController : Controller
    {

        // GET: api/Player
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Player/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Player
        [HttpPost]
        public HttpServiceResult Post([FromBody]string value)
        {
            try
            {
                PlayerDto a = new PlayerDto("Luke");
                
                
                var b  = IResolver.Current.MongoDBRepository.Player_Add(a).Result;
                return new HttpServiceResult { HttpStatus = HttpStatusCode.OK, Message = "OK", ErrorCode = null };
            }
            catch (Exception ex)
            {
                return new HttpServiceResult { HttpStatus = HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }
        
        // PUT: api/Player/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
