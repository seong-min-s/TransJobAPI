using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransJobAPI.Contexts;
using TransJobAPI.Models;

namespace TransJobAPI.Controllers
{
    [Route("api/InitializeMultiChoicesTestAndGetFirstQuestion")]
    [ApiController]
    public class JobDefinitionDepthThreeOrderController : ControllerBase
    {
        public IActionResult Get()
        {
            var db = new InitializeTestDbContext();
            var list = db.JobDefinitionDepthThreeOrders.ToList();
            return Ok(list);
        }
        [HttpPost]
        public IActionResult Post(ExaminationHistory examinationHistory)
        {
            /*depth3이고 사용자 선택에 맞는 세부유형 추출*/
            List<JobDefinition> jobDefinitionDepthThreeList = new List<JobDefinition>();
            if (ModelState.IsValid)
            {
                var db = new InitializeTestDbContext();
                db.ExaminationHistories.Add(examinationHistory);
                db.SaveChanges();

                /*사용자 직무에 맞는 세부유형 직무정의 가져오기*/
                var depthThreeList = db.JobDefinitions.Where(p => p.Depth == 3).OrderBy(p => p.Id).ToList();
                for (int i = 0; i < depthThreeList.Count(); ++i)
                {
                    long id2 = (long)depthThreeList[i].ParentId;
                    long id1 = (long)db.JobDefinitions.Where(p => p.Id == id2).FirstOrDefault().ParentId;
                    long id0 = (long)db.JobDefinitions.Where(p => p.Id == id1).FirstOrDefault().ParentId;
                    if (id0 == examinationHistory.JobDefinitionId)
                    {
                        jobDefinitionDepthThreeList.Add(depthThreeList[i]);
                    }
                }

                /*문제에서 세부유형 직무정의가 일치하고, 개수가 5개 이상인 경우 세부유형 순서 리스트에 넣기*/
                var multipleChoiceQuestionList = db.MultipleChoiceQuestions.ToList();
                var questionJobDefinitionList = db.QuestionJobDefinitions.ToList();
                var jobDefinitionList = db.JobDefinitions.ToList();

                for (int depthThreeListIndex = 0; depthThreeListIndex < jobDefinitionDepthThreeList.Count(); ++depthThreeListIndex)
                {

                    int lv1Count = 0;
                    int lv2Count = 0;
                    int lv3Count = 0;
                    for (int i = 0; i < multipleChoiceQuestionList.Count(); ++i)
                    {
                        long questionId = multipleChoiceQuestionList[i].QuestionId;
                        long questionJobDefinitionId = questionJobDefinitionList.Where(p => p.QuestionId == questionId).FirstOrDefault().JobDefinitionId;

                        if (questionJobDefinitionId == jobDefinitionDepthThreeList[depthThreeListIndex].Id &&
                            multipleChoiceQuestionList[i].QuestionLevel == 1)
                        {
                            ++lv1Count;
                        }
                        else if (questionJobDefinitionId == jobDefinitionDepthThreeList[depthThreeListIndex].Id &&
                            multipleChoiceQuestionList[i].QuestionLevel == 2)
                        {
                            ++lv2Count;
                        }
                        else if (questionJobDefinitionId == jobDefinitionDepthThreeList[depthThreeListIndex].Id &&
                            multipleChoiceQuestionList[i].QuestionLevel == 3)
                        {
                            ++lv3Count;
                        }
                    }

                    if (lv1Count > 4 && lv2Count > 4 && lv3Count > 4)
                    {
                        JobDefinitionDepthThreeOrder order = new JobDefinitionDepthThreeOrder();
                        order.ExaminationHistoryId = examinationHistory.Id;
                        order.JobDefinitionId = jobDefinitionDepthThreeList[depthThreeListIndex].Id;
                        db.JobDefinitionDepthThreeOrders.Add(order);

                        ExamAssignLevel userLevel = new ExamAssignLevel();
                        userLevel.HistoryId = examinationHistory.Id;
                        userLevel.OrderId = order.JobDefinitionId;
                        userLevel.Level = examinationHistory.Level;

                        db.ExamAssignLevels.Add(userLevel);
                    }
                }
                db.SaveChanges();
                var firstOrder = db.JobDefinitionDepthThreeOrders.Where(p => p.ExaminationHistoryId == examinationHistory.Id).FirstOrDefault();

                List<MultipleChoiceQuestion> firstJobDefinitionDepthThreeQuestions = new List<MultipleChoiceQuestion>();
                for (int i = 0; i < multipleChoiceQuestionList.Count(); ++i)
                {
                    //문제의 세부유형직무ID추출
                    long questionId = multipleChoiceQuestionList[i].QuestionId;
                    long questionJobDefinitionId = questionJobDefinitionList.Where(p => p.QuestionId == questionId).FirstOrDefault().JobDefinitionId;

                    //첫 세부유형의 객관식 문제 리스트 생성
                    if (questionJobDefinitionId == firstOrder.JobDefinitionId &&
                        multipleChoiceQuestionList[i].QuestionLevel == examinationHistory.Level)
                    {
                        firstJobDefinitionDepthThreeQuestions.Add(multipleChoiceQuestionList[i]);
                    }
                }

                //첫 세부유형의 객관식 문제 리스트에서 한 개의 문제 랜덤추출
                Random rnd = new Random();
                if (firstJobDefinitionDepthThreeQuestions.Count > 0)
                {
                    int randomIdx = rnd.Next(firstJobDefinitionDepthThreeQuestions.Count());

                    ExaminationHistoryMultipleChoice emc = new ExaminationHistoryMultipleChoice();
                    emc.ExaminationHistoryId = examinationHistory.Id;
                    emc.MultipleQuestionId = firstJobDefinitionDepthThreeQuestions[randomIdx].Id;
                    emc.Seq = 1;
                    emc.Answer = "test";
                    emc.Whether = false;
                    emc.EmployeeId = examinationHistory.EmployeeId;

                    db.ExaminationHistoryMultipleChoices.Add(emc);
                    db.SaveChanges();

                    return Ok(multipleChoiceQuestionList[(int)firstJobDefinitionDepthThreeQuestions[randomIdx].Id - 1]);
                }

                return BadRequest("baddd");
            }

            return Ok();
        }
    }
}
