import React from 'react';
import { Team } from '../../../types/types';
import '../teams.css';

const ShowTeam: React.FC<{ team: Team }> = ({ team }) => {
  return (
    <div className="team-card">
      <div className="team-card-body">
        <div className="team-meta">
          <h3 className="team-name">{team.name}</h3>
        </div>
        {team.logoUrl && (
          <div className="team-image">
            <img src={team.logoUrl} alt={`${team.name} logo`} />
          </div>
        )}
      </div>
    </div>
  );
};

export default ShowTeam;