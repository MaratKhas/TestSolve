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
            Random random = new Random(); //Для генерации рандомного студента
            int maxRandon = countMess.Count;
            //Создание студентов группы
            for(short i = 0; i < countMess.Count; i++)
            {
                students.Add(new Student { Id = i+1, CountMess = countMess[i], CountStudent = countMess.Count, IsKnoing = false });
            }

            //по условию задачи, в начале знает только первый студент
            students[0].IsKnoing = true;

            //рандомная рассылка сообщений

            for (short i = 0; i < students.Count; i++)
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
            }

            //Проверка все ли узнали о тесте
            bool Chek(List<Student> students)
            {
                short chekCount = 0; // переменная для подсчета студентов которые узнали о тесте в ходе программы
                for (short i = 0; i < students.Count; i++)
                {
                    if (students[i].IsKnoing == true)
                        chekCount++;
                }
                if (chekCount == students.Count)
                    return true;
                else
                    return false;
            }                                     
            if (Chek(students))
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