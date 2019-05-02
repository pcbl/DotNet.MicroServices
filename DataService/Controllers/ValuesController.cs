using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            //lets read from Consul!
            return await Packages.Configuration.KeyValue.Get("Values", new string[] { "value1", "value2" });
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            //Test
            var current = await Packages.Configuration.KeyValue.Get("Values", new string[] { "value1", "value2" });
            List<string> items = new List<string>(current);
            var item = "item" + id.ToString();
            items.Add(item);
            var result = await Packages.Configuration.KeyValue.Add("Values", items.ToArray());

            return item;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
