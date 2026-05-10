import React from "react";
import { type Team } from "../../../types/types";

const Team: React.FC<{ team: Team }> = ({ team }) => {
  const [editing, setEditing] = React.useState(false);
  
  return (
    <div>
      <h2>Team Detail</h2>
      <p><strong>Name:</strong> {team.name}</p>
      <p><strong>League:</strong> {team.leagueId}</p>
      <p><strong>Created At:</strong> {team.createdAt}</p>
      
    </div>
  );
};

export default Team;