require "dreams"

player = GetEntity("__player")

interacted = false
startScale = nil
shrinkSeconds = 3

sometimes = { DesertFlower2 = true, DesertFlower3 = true, DesertFlower4 = true }

function start()
    if sometimes[this.GameObject.Name] and not IsDayLinear(3, 1) then
        this.GameObject.SetActive(false)
        return
    end

    if Random.OneIn(2) then
        this.GameObject.Scale = Unity.Vector3(0.5, 0.5, 0.5)
    end
end

function intervalUpdate()
    if not interacted then return end
    local distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if distanceToPlayer < 1.5 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end
end

function update()
    if interacted then return end
    this.Rotate(Unity.Vector3(0, 15, 0))
end

function interact()
    this.LogGraphContribution(6, 2)

    if interacted then return end
    interacted = true
    startScale = this.GameObject.Scale

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0, -0.25))
        .Then(|| this.StopAnimation())
        .Then(shrink)
        .Until(Condition.WaitForSeconds(shrinkSeconds))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end

function shrink()
    local scale = this.GameObject.Scale - startScale * (Unity.DeltaTime() / shrinkSeconds)
    if scale.x < 0 then return end
    this.GameObject.Scale = scale
end