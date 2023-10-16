-- sometimes doesn't run
-- delay before running random
-- speed random

require "dreams"

player = GetEntity("__player")
target = GetEntity("DoublefaceTarget")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 0.25
delay = 0
linked = false
distanceToPlayer = 0

function start()
    if not IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end

    moveSpeed = Random.FloatMinMax(0.4, 0.75)
    delay = Random.FloatMinMax(0, 0.3)
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if linked then return end

    if distanceToPlayer < 0.3 and not linked then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.PitAndTemple)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    if this.GameObject.Name == "DoublefaceA" then
        this.LogGraphContribution(2, 8)
    else
        this.LogGraphContribution(0, 5)
    end

    audio.Play()

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .ThenWaitUntil(Condition.WaitForSeconds(delay))
        .Then(|| this.PlayAnimation(1))
        .Then(|| this.MoveTowards(target.WorldPosition, moveSpeed))
        .Until(Condition.WaitForLinearMove(this.GameObject, target.WorldPosition))
        .ThenFinish()
end