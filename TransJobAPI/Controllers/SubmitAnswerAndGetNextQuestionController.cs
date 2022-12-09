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
            /*객관식 출제 문제 기록 불러오기*/
            var db = new InitializeTestDbContext();
            var orders = db.JobDefinitionDepthThreeOrders.Where(p => p.ExaminationHistoryId == question.ExaminationHistoryId).ToList();
            var examinationHistoryMultipleChoices = db.ExaminationHistoryMultipleChoices.Where(
                p => p.ExaminationHistoryId == question.ExaminationHistoryId
            ).OrderByDescending(p => p.Seq).ToList();
            int num = examinationHistoryMultipleChoices.Count();

            /*정답 체크*/
            {
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

                if (num % MAX != 1)
                {
                    int previousMultipleQuestionId = (int)examinationHistoryMultipleChoices[1].MultipleQuestionId;
                    bool bPreviousIsRight = (bool)examinationHistoryMultipleChoices[1].Whether;
                    var previousQuestion = db.MultipleChoiceQuestions.Where(p => p.Id == previousMultipleQuestionId).FirstOrDefault();

                    int presentMultipleQuestionId = (int)examinationHistoryMultipleChoices[0].MultipleQuestionId;
                    bool bPresentIsRight = (bool)examinationHistoryMultipleChoices[0].Whether;
                    var presentQuestion = db.MultipleChoiceQuestions.Where(p => p.Id == presentMultipleQuestionId).FirstOrDefault();

                    if (previousQuestion.QuestionLevel == previousQuestion.QuestionLevel &&
                        bPreviousIsRight == bPreviousIsRight)
                    {
                        //정답여부에 따른 설정 변경
                    }
                }


                if (num % MAX != 0)
                {
                    goto CREATE_QUESTION;
                }
                else
                {
                    if (num / MAX == 3)
                    {
                        return Ok("Completed");
                    }
                    //다음세부유형으로 진행
                }
            }

        CREATE_QUESTION:
            /*문제출제*/
            {
                //다음 문제의 직무 id 추출
                int depthThreeJobDefinitionId = (int)orders[(num / 5) + 1].JobDefinitionId;
                //get level in multipleChoiceQuestions 이전 문제 정답 여부에 따라 달라지는 코드 추가해야함
                int questionId = (int)examinationHistoryMultipleChoices[0].MultipleQuestionId;
                int nextLevel = (int)db.MultipleChoiceQuestions.Where(p => p.Id == questionId).FirstOrDefault().QuestionLevel;

                /*문제 가져오기 그리고 세부유형과 수준에 맞는 문제를 POOL에 넣기*/
                List<MultipleChoiceQuestion> questionPool = new List<MultipleChoiceQuestion>();
                var questions = db.MultipleChoiceQuestions.ToList();
                for (int i = 0; i < questions.Count(); ++i)
                {
                    long jobDefinitionId = db.QuestionJobDefinitions.Find(questions[i].QuestionId).JobDefinitionId;
                    long level = (long)questions[i].QuestionLevel;
                    if (jobDefinitionId == depthThreeJobDefinitionId
                        && level == nextLevel)
                    {
                        questionPool.Add(questions[i]);
                    }
                }

                Random rnd = new Random();
                int randomIdx = rnd.Next(questionPool.Count());

                ExaminationHistoryMultipleChoice emc = new ExaminationHistoryMultipleChoice
                {
                    ExaminationHistoryId = question.ExaminationHistoryId,
                    MultipleQuestionId = questionPool[randomIdx].Id,
                    Seq = question.Seq + 1,
                    Answer = null,
                    Whether = null,
                    EmployeeId = question.EmployeeId
                };

                db.ExaminationHistoryMultipleChoices.Add(emc);
                db.SaveChanges();

                return Ok(questionPool[randomIdx]);
            }
        }
    }
}
