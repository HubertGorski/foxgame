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

}
