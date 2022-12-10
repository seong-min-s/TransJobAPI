using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransJobAPI.Contexts;
using TransJobAPI.Models;

namespace TransJobAPI.Controllers
{
    [Route("api/SubmitAnswerAndGetNextQuestion")]
    [ApiController]
    public class SubmitAnswerAndGetNextQuestionController : ControllerBase
    {
        const int MAX = 5;
        [HttpPost]
        public IActionResult Post(ExaminationHistoryMultipleChoice question)
        {
            var db = new InitializeTestDbContext();
            var orders = db.JobDefinitionDepthThreeOrders.Where(p => p.ExaminationHistoryId == question.ExaminationHistoryId).ToList();

            int examNum = (int)question.Seq;
            int prevLevel;
            int nowLevel;

            {
                /*채점*/
                long questionId = question.MultipleQuestionId;
                string userAnswer = question.Answer;
                var t = db.ExaminationHistoryMultipleChoices.Find(question.Id);

                if (String.Equals(db.MultipleChoiceQuestions.Find(questionId).RealAnswer, userAnswer) == true)
                {
                    t.Whether = true;
                }
                else
                {
                    t.Whether = false;
                }

                db.SaveChanges();

                if (examNum % 5 != 1)
                {
                    var examinationHistoryMultipleChoices = db.ExaminationHistoryMultipleChoices.Where(
                    p => p.ExaminationHistoryId == question.ExaminationHistoryId).OrderByDescending(p => p.Seq).ToList();

                    int nowMultipleQuestionId = (int)examinationHistoryMultipleChoices[0].MultipleQuestionId;
                    bool bNowIsRight = (bool)examinationHistoryMultipleChoices[0].Whether;
                    var nowQuestion = db.MultipleChoiceQuestions.Where(p => p.Id == nowMultipleQuestionId).FirstOrDefault();
                    nowLevel = (int)nowQuestion.QuestionLevel;
                    
                    int prevMultipleQuestionId = (int)examinationHistoryMultipleChoices[1].MultipleQuestionId;
                    bool bPrevIsRight = (bool)examinationHistoryMultipleChoices[1].Whether;
                    var prevQuestion = db.MultipleChoiceQuestions.Where(p => p.Id == prevMultipleQuestionId).FirstOrDefault();
                    prevLevel = (int)prevQuestion.QuestionLevel;

                    if (prevLevel == nowLevel && bPrevIsRight == bNowIsRight)
                    {
                        var userInfo = db.ExamAssignLevels.Where(p => p.HistoryId == question.ExaminationHistoryId
                        && p.OrderId == orders[(examNum - 1) / 5].JobDefinitionId).FirstOrDefault();

                        if (bNowIsRight == true)
                        {
                            if (userInfo.Level < 3)
                            {
                                ++userInfo.Level;
                            }
                        }
                        else
                        {
                            if (userInfo.Level > 1)
                            {
                                --userInfo.Level;
                            }
                        }
                        db.SaveChanges();
                    }
                }

                if (examNum % MAX != 0)
                {
                    goto CREATE_QUESTION;
                }
                else
                {
                    if (examNum / MAX == orders.Count())
                    {
                        return Ok("Completed");
                    }
                }
            }

        CREATE_QUESTION:
            /*문제출제*/
            {
                int orderIdx = ((examNum + 1 - 1) / 5);
                int depthThreeJobDefinitionId = (int)orders[orderIdx].JobDefinitionId;
                int nextLevel = (int)db.ExamAssignLevels.Where(p => p.HistoryId == question.ExaminationHistoryId
                                    && p.OrderId == depthThreeJobDefinitionId).FirstOrDefault().Level;

                List<MultipleChoiceQuestion> questionPool = new List<MultipleChoiceQuestion>();
                var questions = db.MultipleChoiceQuestions.ToList();
                for (int i = 0; i < questions.Count(); ++i)
                {
                    int jobDefinitionId = (int)db.QuestionJobDefinitions.Where(p => p.QuestionId == questions[i].QuestionId).FirstOrDefault().JobDefinitionId;
                    int level = (int)questions[i].QuestionLevel;

                    if (jobDefinitionId == depthThreeJobDefinitionId
                        && level == nextLevel)
                    {
                        questionPool.Add(questions[i]);
                    }
                }

                Random rnd = new Random();
                int randomIdx = rnd.Next(questionPool.Count());

                ExaminationHistoryMultipleChoice emc = new ExaminationHistoryMultipleChoice();
                emc.ExaminationHistoryId = question.ExaminationHistoryId;
                emc.MultipleQuestionId = questionPool[randomIdx].Id;
                emc.Seq = question.Seq + 1;
                emc.Answer = null;
                emc.Whether = null;
                emc.EmployeeId = question.EmployeeId;

                db.ExaminationHistoryMultipleChoices.Add(emc);
                db.SaveChanges();

                return Ok(questionPool[randomIdx]);
            }
        }
    }
}
