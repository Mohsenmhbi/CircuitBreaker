using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CircuitBreaker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
       
            static int counter = 0;
            [HttpGet("Index")]
            public IActionResult Index()
            {
                return Ok(new Message
                {
                    Id = 1,
                    Text = "درخواست با موفقیت انجام شد"
                });
            }

        [HttpGet("Odd")]
        public IActionResult Odd()
            {
                counter += 1;
                if (counter % 2 != 0)
                {
                    return Ok(new Message
                    {
                        Id = counter,
                        Text = "درخواست در دفعات فرد با موفقیت انجام شد"
                    });
                }
                return BadRequest(new Message
                {
                    Id = counter,
                    Text = "به علت زوج بودن شماره درخواست عملیات ناموفق بود"
                });

            }

        [HttpGet("Ten")]
        public IActionResult Ten()
            {
                counter += 1;
                if (counter % 10 == 0)
                {
                    return Ok(new Message
                    {
                        Id = counter,
                        Text = "درخواست در دفعات ضریب ۱۰ با موفقیت انجام شد"
                    });
                }
                return BadRequest(new Message
                {
                    Id = counter,
                    Text = "به علت ضریب ۱۰ نبودن شماره درخواست عملیات ناموفق بود"
                });

            }
        [HttpGet("Long")]
        public IActionResult Long()
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                System.Threading.Thread.Sleep(20000);
                stopwatch.Stop();

                return Ok(new Message
                {
                    Id = 3,
                    Text = $"درخواست طولانی مدت انجام شد. زمان انجام کار {stopwatch.ElapsedMilliseconds / 1000} ثانیه"
                });
            }

        [HttpGet("Ex")]
        public IActionResult Ex()
            {
                throw new Exception("Exception in Application");
                return Ok(new Message
                {
                    Id = 1,
                    Text = "درخواست با موفقیت انجام شد"
                });
            }
        }
    }

