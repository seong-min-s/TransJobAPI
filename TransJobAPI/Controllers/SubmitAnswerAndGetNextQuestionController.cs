using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public ActionResult<ExaminationHistoryMultipleChoiceDTO> Post(ExaminationHistoryMultipleChoiceDTO question)
        {
            var db = new InitializeTestDbContext();
            var examinationHistory = db.ExaminationHistories.Find(question.ExaminationHistoryId);
            
            if (ModelState.IsValid == false)
            {
                return BadRequest("answer is InValid");
            }
            else if (examinationHistory.IsComplete == true)
            {
                return Ok("Completed.");
            }

            var orders = db.JobDefinitionDepthThreeOrders.Where(p => p.ExaminationHistoryId == question.ExaminationHistoryId).ToList();
            var examinationHistoryMultipleChoices = db.ExaminationHistoryMultipleChoices.Where(
                    p => p.ExaminationHistoryId == question.ExaminationHistoryId).OrderByDescending(p => p.Seq).ToList();


            int examNum = (int)examinationHistoryMultipleChoices[0].Seq;
            int prevLevel;
            int nowLevel;
            /*채점과 등급 조정*/
            {
                
                long questionId = question.MultipleQuestionId;
                string userAnswer = question.Answer;
                var t = db.ExaminationHistoryMultipleChoices.Where(p => p.ExaminationHistoryId == question.ExaminationHistoryId && p.MultipleQuestionId == question.MultipleQuestionId).FirstOrDefault();


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
                        db.ExaminationHistories.Find(question.ExaminationHistoryId).IsComplete = true;
                        db.SaveChanges();
                        return Ok("Completed");
                    }
                }
            }

        CREATE_QUESTION:
            /*문제출제*/
            {
                int orderIdx = examNum / 5;
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
        EXTRACT_RANDOM_IDX:
                Random rnd = new Random();
                int randomIdx = rnd.Next(questionPool.Count());

                /*사용자 답변 기록을 위한 객관식 문제 테이블 테이블 저장용 데이터*/
                ExaminationHistoryMultipleChoice emc = new ExaminationHistoryMultipleChoice
                {
                    EmployeeId = question.EmployeeId,
                    ExaminationHistoryId = question.ExaminationHistoryId,
                    MultipleQuestionId = questionPool[randomIdx].Id,
                    Seq = examNum + 1,
                    Answer = null,
                    Whether = null
                };
                bool bIsDuplicated = db.ExaminationHistoryMultipleChoices.Where(p => p.ExaminationHistoryId == emc.ExaminationHistoryId &&
                p.MultipleQuestionId == emc.MultipleQuestionId).Count() > 0;

                
                if (bIsDuplicated == true)
                {
                    goto EXTRACT_RANDOM_IDX;
                }
                db.ExaminationHistoryMultipleChoices.Add(emc);
                db.SaveChanges();


                StringBuilder sb = new StringBuilder(512);
                sb.Append("#");
                sb.Append(questionPool[randomIdx].Answer1);
                sb.Append("#");
                sb.Append(questionPool[randomIdx].Answer2);
                sb.Append("#");
                sb.Append(questionPool[randomIdx].Answer3);
                sb.Append("#");
                sb.Append(questionPool[randomIdx].Answer4);
                var qJobDefinition = db.QuestionJobDefinitions.Where(p => p.QuestionId == questionPool[randomIdx].QuestionId).FirstOrDefault();
                
                /*사용자 답변 기록을 위한 객관식 문제 테이블 전송용 데이터*/
                ExaminationHistoryMultipleChoiceDTO emcDTO = new ExaminationHistoryMultipleChoiceDTO()
                {

                    EmployeeId = emc.EmployeeId,
                    ExaminationHistoryId = emc.ExaminationHistoryId,
                    MultipleQuestionId = emc.MultipleQuestionId,
                    JobDefinitionId = qJobDefinition.JobDefinitionId,
                    Answer = sb.ToString(),
                    Contents = questionPool[randomIdx].QuestionContents
                };

                return Ok(emcDTO);
            }
        }
    }
}
