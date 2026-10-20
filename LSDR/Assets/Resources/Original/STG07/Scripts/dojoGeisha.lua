require "dreams"

player = GetEntity("__player")
target1 = GetEntity("GeishaTarget1")
target2 = GetEntity("GeishaTarget2")

interacted = false
moveSpeed = 0.2
wanders = false

function start()
    -- the little ones are there every even day, and just stand about
    if this.GameObject.Name ~= "Geisha" then
        if not IsDayEven() then this.GameObject.SetActive(false) end
        return
    end

    if not IsDayEven() or IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end

    wanders = Random.OneIn(3)
    if wanders then
        this.GameObject.Scale = Unity.Vector3(0.5, 0.5, 0.5)
        this.GameObject.WorldPosition = this.GameObject.WorldPosition + Unity.Vector3(0, 0.15, 0)
    end
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.4 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Kyoto)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    this.LogGraphContribution(2, 2)

    if wanders and not interacted then
        this.Action
            .Do(|| this.LookAtPlane(target1.WorldPosition))
            .Then(|| this.MoveTowards(target1.WorldPosition, moveSpeed))
            .Until(Condition.WaitForLinearMove(this.GameObject, target1.WorldPosition))
            .Then(|| this.LookAtPlane(target2.WorldPosition))
            .Then(|| this.MoveTowards(target2.WorldPosition, moveSpeed))
            .Until(Condition.WaitForLinearMove(this.GameObject, target2.WorldPosition))
            .ThenFinish()
    end

    interacted = true
end