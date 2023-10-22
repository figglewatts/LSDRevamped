require "dreams"

player = GetEntity("__player")
sumoTarget = GetEntity("SumoTarget")
sumoStrikeAudio = GetEntity("SumoStrikeAudio").DreamAudio
sumoFootstepAudio = GetEntity("SumoFootstepAudio").DreamAudio

interacted = false
moveSpeed = 1.75

function start()
    if not IsDayLinear(3, 1) and not IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.5 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.MonumentPark)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    if this.GameObject.Name == "SumoA" then
        this.LogGraphContribution(-2, 3)
    else
        this.LogGraphContribution(2, -3)
    end
    interacted = true

    this.PlayAnimation(0)
    sumoFootstepAudio.Play()
    local animLength = this.GetAnimationLengthSeconds(0)

    this.Action
        .WaitUntil(Condition.WaitForSeconds(animLength / 4))
        .Then(|| sumoFootstepAudio.Stop())
        .Then(|| this.StopAnimation())
        .Then(|| this.MoveInDirection(this.Forward.negated(), moveSpeed))
        .Until(Condition.WaitForSeconds(0.5))
        .Then(|| sumoStrikeAudio.Play())
        .Then(|| this.MoveInDirection(this.Forward, moveSpeed))
        .Until(Condition.WaitForSeconds(0.5))
        .Then(|| this.ResumeAnimation())
        .Then(|| sumoFootstepAudio.Play())
        .ThenWaitUntil(Condition.WaitForSeconds(animLength / 4))
        .ThenLoop()
end