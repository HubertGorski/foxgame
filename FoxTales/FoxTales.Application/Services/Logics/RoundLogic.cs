using FoxTales.Application.DTOs.Psych;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Interfaces.Logics;

namespace FoxTales.Application.Services.Logics;

public class RoundLogic : IRoundLogic
{

    public QuestionDto GetNewCurrentQuestionWithSelectedPlayer(RoomDto room)
    {
        if (room.Questions.Count == 0)
            throw new InvalidOperationException($"Question does not exist in room '{room.Code}'. (GetNewCurrentQuestionWithSelectedPlayer)");

        if (room.Users.Count == 0)
            throw new InvalidOperationException($"Room '{room.Code}' is empty. (GetNewCurrentQuestionWithSelectedPlayer)");

        int minSelectionCount = room.Users.Min(u => u.SelectionCount);
        List<PlayerDto> eligibleUsers = [.. room.Users.Where(u => u.SelectionCount == minSelectionCount)];

        Random rnd = Random.Shared;
        QuestionDto currentQuestion = room.Questions[rnd.Next(room.Questions.Count)];
        PlayerDto selectedPlayer = eligibleUsers[rnd.Next(eligibleUsers.Count)];

        currentQuestion.CurrentUser = selectedPlayer;
        selectedPlayer.SelectionCount += 1;

        if (currentQuestion.Text.Contains("****")) // TODO: zrobic wykrywanie gracza.
            currentQuestion.Text = currentQuestion.Text.Replace("****", selectedPlayer.Username);

        return currentQuestion;
    }

    public void UpdateVotePool(PlayerDto voter, PlayerDto owner)
    {
        if (owner.Answer == null)
            throw new InvalidOperationException($"Player {owner.UserId} not provide answer. (UpdateVotePool)");

        if (owner.VotersIdsForHisAnswer.Contains(voter.UserId))
            return;

        owner.Answer.VotersCount++;
        owner.VotersIdsForHisAnswer.Add(voter.UserId);
        int voterIndex = owner.VotesReceived.FindIndex(kv => kv.Key == voter.UserId);

        if (voterIndex >= 0)
            owner.VotesReceived[voterIndex] = new KeyValuePair<int, int>(voter.UserId, owner.VotesReceived[voterIndex].Value + 1);
        else
            owner.VotesReceived.Add(new KeyValuePair<int, int>(voter.UserId, 1));

        int index = voter.VotesGiven.FindIndex(kv => kv.Key == owner.UserId);

        if (index >= 0)
            voter.VotesGiven[index] = new KeyValuePair<int, int>(owner.UserId, voter.VotesGiven[index].Value + 1);
        else
            voter.VotesGiven.Add(new KeyValuePair<int, int>(owner.UserId, 1));
    }

    public void AssignPoints(RoomDto room, PlayerDto answerOwner)
    {
        if (room.UseDixitRules)
        {
            AssignPointsByDixitRules(room);
        }
        else
        {
            AssignPointsByPsychRules(answerOwner);
        }
    }

    private static void AssignPointsByPsychRules(PlayerDto answerOwner)
    {
        answerOwner.PointsInGame += 10;
    }

    private static void AssignPointsByDixitRules(RoomDto room)
    {
        if (room.Users.Any(u => !u.IsReady))
        {
            return;
        }

        if (room.CurrentQuestion == null || room.CurrentQuestion.CurrentUser == null)
        {
            throw new InvalidOperationException($"Question does not exist in room '{room.Code}'.");
        }

        PlayerDto subjectUser = room.CurrentQuestion.CurrentUser;
        List<int> winningUsersIds = subjectUser.VotersIdsForHisAnswer;
        winningUsersIds.Remove(subjectUser.UserId);

        List<PlayerDto> allPlayersExceptSubjectUser = [.. room.Users.Where(u => u.UserId != subjectUser.UserId)];
        if (winningUsersIds.Count > 0 && winningUsersIds.Count < allPlayersExceptSubjectUser.Count)
        {
            subjectUser.PointsInGame += 10 * winningUsersIds.Count;
        }

        room.Users
            .Where(u => winningUsersIds.Contains(u.UserId))
            .ToList()
            .ForEach(user => user.PointsInGame += 10);

        PlayerDto? selectedUserBySubjectUser = room.Users.Find(u => u.VotersIdsForHisAnswer.Contains(subjectUser.UserId));
        if (selectedUserBySubjectUser != null)
        {
            selectedUserBySubjectUser.PointsInGame += 10;
        }
    }

    /* TODO: add new game type
        public void AssignPointsByTeamRules(RoomDto room)
        {
            if (room.Users.Any(u => !u.IsReady))
            {
                return;
            }

            if (room.CurrentQuestion == null || room.CurrentQuestion.CurrentUser == null)
            {
                throw new InvalidOperationException($"Question does not exist in room '{room.Code}'.");
            }

            PlayerDto subjectUser = room.CurrentQuestion.CurrentUser;

            List<PlayerDto> usersWithSameAnswer = room.Users?
                .Where(u => AreAnswersTheSame(u.Answer, subjectUser.Answer) && u.UserId != subjectUser.UserId)
                .ToList() ?? [];

            foreach (PlayerDto user in usersWithSameAnswer)
            {
                user.PointsInGame += 10;
            }

            Dictionary<int, List<PlayerDto>> teams = room.Users?
                .Where(u => u.TeamId.HasValue)
                .GroupBy(u => u.TeamId.Value)
                .ToDictionary(g => g.Key, g => g.ToList())
                ?? [];

            if (subjectUser.TeamId == null || !teams.TryGetValue(subjectUser.TeamId.Value, out List<PlayerDto>? teamMembers))
                return;

            PlayerDto teammate = teamMembers.FirstOrDefault(u => u.UserId != subjectUser.UserId) ?? throw new InvalidOperationException($"Teammate does not exist in room '{room.Code}'.");

            if (AreAnswersTheSame(teammate.Answer, subjectUser.Answer))
            {
                subjectUser.PointsInGame += 10;
            }
        }
    */

}
