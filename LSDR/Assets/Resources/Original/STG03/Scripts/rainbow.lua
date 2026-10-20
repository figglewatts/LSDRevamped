turning = false

function start()
    if DreamSystem.EnvironmentGroup % 2 ~= 0 then
        this.GameObject.SetActive(false)
        return
    end

    if ChartZone() == "white" then
        this.GameObject.Scale = Unity.Vector3(3, 3, 3)
        this.GameObject.WorldPosition = this.GameObject.WorldPosition + Unity.Vector3(0, 15, 0)
    end

    turning = Random.OneIn(5)
end

function update()
    if not turning then return end
    this.Rotate(Unity.Vector3(0, 2, 0))
end

function interact()
    this.LogGraphContribution(-8, 1)
end