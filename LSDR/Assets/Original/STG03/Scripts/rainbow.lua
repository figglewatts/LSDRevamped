function start()
    if Random.OneIn(2) then
        this.GameObject.Scale = Unity.Vector3(2, 2, 2)
    end
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(-8, 1)
end