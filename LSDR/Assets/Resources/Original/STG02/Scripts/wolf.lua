player = GetEntity("__player")

glideSpeed = 0.9
runSpeed = 3.5
interacted = false
running = false
racing = false

function start()
    this.SetChildVisible(false)
    
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    if not interacted then return end

    if racing then
        -- races round in circles, flickering in and out of sight
        this.Rotate(Unity.Vector3(0, 360, 0))
        this.MoveInDirection(this.Forward, 20)
        this.SetRenderersActive(Random.OneIn(7))
        return
    end

    local speed = glideSpeed
    if running then speed = runSpeed end
    this.MoveInDirection(this.Forward, speed)
end

function interact()
    if interacted then return end
    interacted = true
    this.SetChildVisible(true)
    this.PlayAnimation(0)

    this.LogGraphContribution(-3, 2)

    racing = Random.OneIn(2)
    if racing then
        this.Action
            .WaitUntil(Condition.WaitForSeconds(18))
            .Then(|| this.GameObject.SetActive(false))
            .ThenFinish()
        return
    end

    local turn = 90
    if Random.OneIn(2) then turn = -turn end

    this.Action
        .WaitUntil(Condition.WaitForSeconds(3))
        .Then(function()
            local rotation = this.GameObject.LocalRotation
            rotation.y = rotation.y + turn
            this.GameObject.LocalRotation = rotation
            running = true
        end)
        .ThenWaitUntil(Condition.WaitForSeconds(15))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end