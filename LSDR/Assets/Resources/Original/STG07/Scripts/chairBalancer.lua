require "dreams"

player = GetEntity("__player")
audio = GetEntity("ChairBalancerAudio").DreamAudio

interacted = false

function start()
    if IsDayEven() or IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end

    this.Action
        .Do(|| audio.Play())
        .ThenWaitUntil(Condition.WaitForSeconds(1.5))
        .ThenLoop()
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.25 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.TempleDojo)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(-4, -1)
    interacted = true
end