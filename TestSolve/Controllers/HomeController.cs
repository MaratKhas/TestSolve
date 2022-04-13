using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestSolve.Models;

namespace TestSolve.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //Гет запросы для навигации в приложении
        public IActionResult Preview() => View();

        public IActionResult StudentCount(Student student) => View(student);

        //Пост запросы для реализации логики
        [HttpPost]
        public IActionResult MessagesCount(Student student)
        {
            return View(student);
        }

        [HttpPost]
        public IActionResult Result(List<int> countMess)
        {
            List<Student> students = new List<Student>(countMess.Count);
            Ways ways = new Ways(countMess.Count);
            //Создание студентов группы
            for (short i = 0; i < countMess.Count; i++)
            {
                students.Add(new Student { Id = i + 1, CountMess = countMess[i], CountStudent = countMess.Count, IsKnoing = false });
            }

            //по условию задачи, в начале знает только первый студент
            students[0].IsKnoing = true;

            //рандомная рассылка сообщений

            /* for (short i = 0; i < students.Count; i++)
             {
                 //Рассылка сообщений от i-го студента
                 if (students[i].IsKnoing == true && students[i].CountMess > 0)
                 {
                     //Рассылка сообщений другим рандомным студентам
                     while (students[i].CountMess > 0 )
                     {
                         if (Chek(students))
                             break;
                         int rand = random.Next(0, maxRandon);
                         if (i != rand && ways.way[rand,i] == 0 && ways.way[i,rand] == 0)//Для пропуска момента отправки себе,тому от кого узнал о тесте, и повторного отправления одному и тому же студенту
                         {
                             students[i].CountMess--;
                             students[rand].IsKnoing = true;
                             ways.way[i, rand] += 1;
                         }
                         else
                             continue;
                     }
                 }
                 else
                     continue;
             }*/
            //Рассылка сообщений до тех пор пока она не осуществиться если это возможно
            
            if ( ChekCountMesseges(students) && students[0].CountMess > 0)
            {
                for (short i = 0; i < students.Count; i++)
                {
                    if (ChekKnowledge(students)) return View("Result", ways);

                    if (students[i].CountMess > 0 && students[i].IsKnoing == true)
                    {
                        if (ChekKnowledge(students)) return View("Result", ways);
                        for (short k = 0; k < 2; k++ )//В первой итерации отправляется только тем кто знает, во второй оставшиеся сообщения
                        {                            
                            for (short j = 0; j < students.Count; j++)
                            {
                                switch (k)
                                {
                                    case 0:
                                        SendToBeSend(students,ways, i, j); 
                                        break;
                                    case 1:
                                        SendAll(students, ways, i, j);
                                        break;
                                }
                            }

                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                return View("NullResult");
            }

            void SendToBeSend(List<Student> students,Ways ways, int i, int j)
            {
                if (ChekRepitWays(ways, i, j) && students[i].CountMess > 0 && students[j].CountMess > 0)
                {
                    students[i].CountMess--;
                    students[j].IsKnoing = true;
                    ways.way[i, j] += 1;
                }
               
            }

            void SendAll(List<Student> students, Ways ways, int i, int j)
            {
                if (ChekRepitWays(ways, i, j) && students[i].CountMess > 0 && students[j].IsKnoing == false)
                {
                    students[i].CountMess--;
                    students[j].IsKnoing = true;
                    ways.way[i, j] += 1;
                }

            }

            //Проверка все ли узнали о тесте
            bool ChekKnowledge(List<Student> students)
            {
                if (students.All(x => x.IsKnoing == true))
                    return true;
                else
                    return false;
            }

            //Для пропуска момента отправки себе,тому от кого узнал о тесте, и повторного отправления одному и тому же студенту
            bool ChekRepitWays(Ways ways, int i, int j)
            {
                if (i != j && ways.way[j, i] == 0 && ways.way[i, j] == 0)
                    return true;

                else return false;
            }

            //Проверка хвтит ли сообщений для рассылки
            bool ChekCountMesseges(List<Student> students)
            {
                var AllMess = students.Select(x => x.CountMess).Sum();
                var NeedMess = students[0].CountStudent - 1;
                if (AllMess >= NeedMess)
                    return true;
                else
                    return false;
            }

            if (ChekKnowledge(students))
                return View(ways);
            else
                return View("NullResult", ways);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
           return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    } 
} 
